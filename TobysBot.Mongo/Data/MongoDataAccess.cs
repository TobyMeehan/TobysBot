using MongoDB.Driver;
using TobysBot.Data;
using TobysBot.Mongo.Client;

namespace TobysBot.Mongo.Data;

public class MongoDataAccess : IDataAccess
{
    private readonly IMongoService _service;

    public MongoDataAccess(IMongoService service)
    {
        _service = service;
    }

    public async Task<T> SaveAsync<T>(string collectionName, T data) where T : IEntity
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq(x => x.Id, data.Id);

        await collection.ReplaceOneAsync(filter, data, new ReplaceOptions {IsUpsert = true});

        return data;
    }

    public async Task<IReadOnlyCollection<T>> GetAsync<T>(string collectionName) where T : IEntity
    {
        var collection = _service.Connect<T>(collectionName);

        var result = await collection.FindAsync(_ => true);

        return result.ToList();
    }

    public async Task<T> GetAsync<T>(string collectionName, string id) where T : IEntity
    {
        var collection = _service.Connect<T>(collectionName);

        var result = await collection.FindAsync(x => x.Id == id);

        return result.FirstOrDefault();
    }

    public async Task<T> GetAsync<T>(string collectionName, ulong id) where T : IDiscordEntity
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq("DiscordId", id);

        var result = await collection.FindAsync(filter);

        return result.FirstOrDefault();
    }

    public async Task<T> GetByNameAsync<T>(string collectionName, string name) where T : INamedEntity
    {
        var collection = _service.Connect<T>(collectionName);

        var result = await collection.FindAsync(x => x.Name == null);

        return result.FirstOrDefault();
    }

    public async Task DeleteAsync<T>(string collectionName, T data) where T : IEntity
    {
        var collection = _service.Connect<T>(collectionName);

        await collection.DeleteOneAsync(x => x.Id == data.Id);
    }
}