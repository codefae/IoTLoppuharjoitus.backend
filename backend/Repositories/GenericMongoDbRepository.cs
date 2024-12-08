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

    public async Task AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task<List<T>> FindAsync(FilterDefinition<T> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }
    
    public async Task<List<T>> FindAsync(FilterDefinition<T> filter, SortDefinition<T> sort)
    {
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }
}