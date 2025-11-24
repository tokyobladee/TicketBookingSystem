using TicketBookingSystem.Domain.Entities;
using TicketBookingSystem.Infrastructure.Data;

namespace TicketBookingSystem.Infrastructure.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

public class UserRepository : MongoRepository<User>, IUserRepository
{
    public UserRepository(IMongoDbContext context)
        : base(context, "users")
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await FindOneAsync(u => u.Email == email);
    }
}
