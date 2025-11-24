using TicketBookingSystem.Domain.Entities;
using TicketBookingSystem.Infrastructure.Data;

namespace TicketBookingSystem.Infrastructure.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Booking>> GetByShowtimeIdAsync(string showtimeId);
}

public class BookingRepository : MongoRepository<Booking>, IBookingRepository
{
    public BookingRepository(IMongoDbContext context)
        : base(context, "bookings")
    {
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId)
    {
        return await FindAsync(b => b.UserId == userId);
    }

    public async Task<IEnumerable<Booking>> GetByShowtimeIdAsync(string showtimeId)
    {
        return await FindAsync(b => b.ShowtimeId == showtimeId);
    }
}
