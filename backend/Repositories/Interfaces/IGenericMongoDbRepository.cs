using MongoDB.Driver;

namespace backend.Repositories.Interfaces;

public interface IGenericMongoDbRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(string id);
    Task AddAsync(T entity);
    Task UpdateAsync(string id, T entity);
    Task DeleteAsync(string id);
    Task<List<T>> FindAsync(FilterDefinition<T> filter);
    Task<List<T>> FindAsync(FilterDefinition<T> filter, SortDefinition<T> sort);
}