using MongoDB.Driver;
using TicketBookingSystem.Domain.Entities;
using TicketBookingSystem.Domain.Enums;
using TicketBookingSystem.Infrastructure.Data;
using TicketBookingSystem.Infrastructure.Repositories;

namespace TicketBookingSystem.Infrastructure.Services;

public interface IBookingService
{
    Task<(bool Success, Booking? Booking, string Message)> CreateBookingAsync(string userId, string showtimeId, List<string> seatNumbers);
    Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId);
    Task<(bool Success, string Message)> CancelBookingAsync(string bookingId, string userId);
    Task<List<string>> GetAvailableSeatsAsync(string showtimeId);
}

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IShowtimeRepository _showtimeRepository;
    private readonly IHallRepository _hallRepository;
    private readonly IMongoDbContext _context;
    private const int MaxRetries = 3;

    public BookingService(
        IBookingRepository bookingRepository,
        IShowtimeRepository showtimeRepository,
        IHallRepository hallRepository,
        IMongoDbContext context)
    {
        _bookingRepository = bookingRepository;
        _showtimeRepository = showtimeRepository;
        _hallRepository = hallRepository;
        _context = context;
    }

    public async Task<(bool Success, Booking? Booking, string Message)> CreateBookingAsync(
        string userId, string showtimeId, List<string> seatNumbers)
    {
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(showtimeId);
            if (showtime == null)
            {
                return (false, null, "Showtime not found");
            }

            var unavailableSeats = seatNumbers.Where(s => showtime.BookedSeats.Contains(s)).ToList();
            if (unavailableSeats.Any())
            {
                return (false, null, $"Seats already booked: {string.Join(", ", unavailableSeats)}");
            }

            var hall = await _hallRepository.GetByIdAsync(showtime.HallId);
            if (hall == null)
            {
                return (false, null, "Hall not found");
            }

            foreach (var seat in seatNumbers)
            {
                if (!IsValidSeat(seat, hall))
                {
                    return (false, null, $"Invalid seat: {seat}");
                }
            }

            using var session = await _context.Client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var updateSuccess = await _showtimeRepository.BookSeatsAsync(
                    showtimeId, seatNumbers, showtime.Version);

                if (!updateSuccess)
                {
                    await session.AbortTransactionAsync();
                    if (attempt < MaxRetries - 1)
                    {
                        await Task.Delay(100 * (attempt + 1));
                        continue;
                    }
                    return (false, null, "Booking conflict. Please try again.");
                }

                var booking = new Booking
                {
                    UserId = userId,
                    ShowtimeId = showtimeId,
                    SeatNumbers = seatNumbers,
                    TotalPrice = showtime.Price * seatNumbers.Count,
                    Status = BookingStatus.Confirmed,
                    BookedAt = DateTime.UtcNow
                };

                await _bookingRepository.CreateAsync(booking);
                await session.CommitTransactionAsync();

                return (true, booking, "Booking created successfully");
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                if (attempt < MaxRetries - 1)
                {
                    await Task.Delay(100 * (attempt + 1));
                    continue;
                }
                return (false, null, $"Booking failed: {ex.Message}");
            }
        }

        return (false, null, "Maximum retry attempts exceeded");
    }

    public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId)
    {
        return await _bookingRepository.GetByUserIdAsync(userId);
    }

    public async Task<(bool Success, string Message)> CancelBookingAsync(string bookingId, string userId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return (false, "Booking not found");
        }

        if (booking.UserId != userId)
        {
            return (false, "Unauthorized");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return (false, "Booking already cancelled");
        }

        var showtime = await _showtimeRepository.GetByIdAsync(booking.ShowtimeId);
        if (showtime == null)
        {
            return (false, "Showtime not found");
        }

        if (showtime.StartTime <= DateTime.UtcNow.AddHours(2))
        {
            return (false, "Cannot cancel booking less than 2 hours before showtime");
        }

        using var session = await _context.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var update = Builders<Showtime>.Update.PullAll(s => s.BookedSeats, booking.SeatNumbers);
            await _showtimeRepository.UpdateWithVersionAsync(booking.ShowtimeId, update, showtime.Version);

            booking.Status = BookingStatus.Cancelled;
            await _bookingRepository.UpdateAsync(bookingId, booking);

            await session.CommitTransactionAsync();
            return (true, "Booking cancelled successfully");
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            return (false, $"Cancellation failed: {ex.Message}");
        }
    }

    public async Task<List<string>> GetAvailableSeatsAsync(string showtimeId)
    {
        var showtime = await _showtimeRepository.GetByIdAsync(showtimeId);
        if (showtime == null)
        {
            return new List<string>();
        }

        var hall = await _hallRepository.GetByIdAsync(showtime.HallId);
        if (hall == null)
        {
            return new List<string>();
        }

        var allSeats = new List<string>();
        for (int row = 1; row <= hall.Rows; row++)
        {
            for (int seat = 1; seat <= hall.SeatsPerRow; seat++)
            {
                allSeats.Add($"{row}-{seat}");
            }
        }

        return allSeats.Except(showtime.BookedSeats).ToList();
    }

    private bool IsValidSeat(string seat, Hall hall)
    {
        var parts = seat.Split('-');
        if (parts.Length != 2)
            return false;

        if (!int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int seatNum))
            return false;

        return row >= 1 && row <= hall.Rows && seatNum >= 1 && seatNum <= hall.SeatsPerRow;
    }
}
