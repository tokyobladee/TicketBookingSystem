using MongoDB.Driver;
using System.Linq.Expressions;
using TicketBookingSystem.Infrastructure.Data;

namespace TicketBookingSystem.Infrastructure.Repositories;

public class MongoRepository<T> : IRepository<T> where T : class
{
    protected readonly IMongoCollection<T> _collection;
    protected readonly IMongoDbContext _context;

    public MongoRepository(IMongoDbContext context, string collectionName)
    {
        _context = context;
        _collection = context.GetCollection<T>(collectionName);
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    public virtual async Task<T?> FindOneAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public virtual async Task<bool> UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));
        var result = await _collection.ReplaceOneAsync(filter, entity);
        return result.ModifiedCount > 0;
    }

    public virtual async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));
        var result = await _collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public virtual async Task<bool> UpdateWithVersionAsync(string id, UpdateDefinition<T> update, long expectedVersion)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id)),
            Builders<T>.Filter.Eq("version", expectedVersion)
        );

        var updateWithVersion = Builders<T>.Update
            .Combine(update, Builders<T>.Update.Inc("version", 1));

        var result = await _collection.UpdateOneAsync(filter, updateWithVersion);
        return result.ModifiedCount > 0;
    }
}
