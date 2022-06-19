using MongoDB.Driver;

namespace TobysBot.Mongo.Client;

public interface IMongoService
{
    IMongoCollection<T> Connect<T>(in string collectionName);
}