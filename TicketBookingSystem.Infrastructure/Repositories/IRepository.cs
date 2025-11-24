using MongoDB.Driver;
using System.Linq.Expressions;

namespace TicketBookingSystem.Infrastructure.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
    Task<T> CreateAsync(T entity);
    Task<bool> UpdateAsync(string id, T entity);
    Task<bool> DeleteAsync(string id);
    Task<bool> UpdateWithVersionAsync(string id, UpdateDefinition<T> update, long expectedVersion);
    Task<T?> FindOneAsync(Expression<Func<T, bool>> filter);
}
