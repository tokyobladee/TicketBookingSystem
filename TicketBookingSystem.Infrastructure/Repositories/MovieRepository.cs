using TicketBookingSystem.Domain.Entities;
using TicketBookingSystem.Infrastructure.Data;

namespace TicketBookingSystem.Infrastructure.Repositories;

public interface IMovieRepository : IRepository<Movie>
{
    Task<IEnumerable<Movie>> GetByGenreAsync(string genre);
    Task<Movie?> GetByTitleAsync(string title);
}

public class MovieRepository : MongoRepository<Movie>, IMovieRepository
{
    public MovieRepository(IMongoDbContext context)
        : base(context, "movies")
    {
    }

    public async Task<IEnumerable<Movie>> GetByGenreAsync(string genre)
    {
        return await FindAsync(m => m.Genre == genre);
    }

    public async Task<Movie?> GetByTitleAsync(string title)
    {
        return await FindOneAsync(m => m.Title == title);
    }
}
