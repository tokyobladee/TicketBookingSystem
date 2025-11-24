using MongoDB.Driver;

namespace TicketBookingSystem.Infrastructure.Data;

public interface IMongoDbContext
{
    IMongoDatabase Database { get; }
    IMongoClient Client { get; }
    IMongoCollection<T> GetCollection<T>(string collectionName);
}
