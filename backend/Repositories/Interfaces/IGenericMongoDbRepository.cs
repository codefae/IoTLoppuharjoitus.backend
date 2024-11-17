using System.Linq.Expressions;

namespace backend.Repositories.Interfaces;

public interface IGenericMongoDbRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(string id);
    Task AddAsync(T entity);
    Task UpdateAsync(string id, T entity);
    Task DeleteAsync(string id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
}