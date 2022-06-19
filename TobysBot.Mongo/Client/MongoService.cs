using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TobysBot.Mongo.Configuration;

namespace TobysBot.Mongo.Client;

public class MongoService : IMongoService
{
    private readonly MongoOptions _options;

    public MongoService(IOptions<MongoOptions> options)
    {
        _options = options.Value;
    }
    
    public IMongoCollection<T> Connect<T>(in string collectionName)
    {
        var client = new MongoClient(_options.ConnectionString);
        var db = client.GetDatabase(_options.DatabaseName);
        return db.GetCollection<T>(collectionName);
    }
}