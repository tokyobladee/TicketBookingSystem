using MongoDB.Driver;
using TicketBookingSystem.Domain.Entities;
using TicketBookingSystem.Infrastructure.Data;

namespace TicketBookingSystem.Infrastructure.Repositories;

public interface IShowtimeRepository : IRepository<Showtime>
{
    Task<IEnumerable<Showtime>> GetByMovieIdAsync(string movieId);
    Task<IEnumerable<Showtime>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> BookSeatsAsync(string showtimeId, List<string> seats, long expectedVersion);
}

public class ShowtimeRepository : MongoRepository<Showtime>, IShowtimeRepository
{
    public ShowtimeRepository(IMongoDbContext context)
        : base(context, "showtimes")
    {
    }

    public async Task<IEnumerable<Showtime>> GetByMovieIdAsync(string movieId)
    {
        return await FindAsync(s => s.MovieId == movieId);
    }

    public async Task<IEnumerable<Showtime>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await FindAsync(s => s.StartTime >= startDate && s.StartTime <= endDate);
    }

    public async Task<bool> BookSeatsAsync(string showtimeId, List<string> seats, long expectedVersion)
    {
        var update = Builders<Showtime>.Update.PushEach(s => s.BookedSeats, seats);
        return await UpdateWithVersionAsync(showtimeId, update, expectedVersion);
    }
}
