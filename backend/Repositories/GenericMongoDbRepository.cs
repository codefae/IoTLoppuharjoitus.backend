using System.Linq.Expressions;
using backend.Models;
using backend.Repositories.Interfaces;
using backend.settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Repositories;

public class GenericMongoDbRepository<T> : IGenericMongoDbRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public GenericMongoDbRepository(IOptions<DatabaseSettings<IotData>> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<T>(settings.Value.CollectionName);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T> GetByIdAsync(string id)
    {
        return await _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(string id, T entity)
    {
        await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), entity);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }
}