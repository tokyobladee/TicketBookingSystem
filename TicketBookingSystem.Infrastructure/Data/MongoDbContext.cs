using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TicketBookingSystem.Infrastructure.Data;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly IMongoClient _client;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        _client = new MongoClient(settings.Value.ConnectionString);
        _database = _client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoDatabase Database => _database;
    public IMongoClient Client => _client;

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}
