using MongoDB.Driver;

namespace backend.Repositories.Interfaces;

public interface IGenericMongoDbRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task<List<T>> FindAsync(FilterDefinition<T> filter);
    Task<List<T>> FindAsync(FilterDefinition<T> filter, SortDefinition<T> sort);
}