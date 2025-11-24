using TicketBookingSystem.Domain.Entities;
using TicketBookingSystem.Infrastructure.Data;

namespace TicketBookingSystem.Infrastructure.Repositories;

public interface IHallRepository : IRepository<Hall>
{
}

public class HallRepository : MongoRepository<Hall>, IHallRepository
{
    public HallRepository(IMongoDbContext context)
        : base(context, "halls")
    {
    }
}
