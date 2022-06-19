using Discord;
using MongoDB.Driver;
using TobysBot.Data;
using TobysBot.Mongo.Client;

namespace TobysBot.Mongo.Data;

public class MongoDataAccess : IDataAccess
{
    private readonly IMongoService _service;

    private class Fields
    {
        public const string Name = "Name";
        public const string DiscordId = "DiscordId";
        public const string GuildId = "GuildId";
        public const string UserId = "UserId";
        public const string ChannelId = "ChannelId";
    }
    
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

        var filter = Builders<T>.Filter.Eq(Fields.DiscordId, id);

        var result = await collection.FindAsync(filter);

        return result.FirstOrDefault();
    }

    public async Task<IReadOnlyCollection<T>> GetByNameAsync<T>(string collectionName, string name) where T : INamedEntity
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq(Fields.Name, name);

        var result = await collection.FindAsync(filter);

        return result.ToList();
    }

    public async Task<IReadOnlyCollection<T>> GetByGuildAsync<T>(string collectionName, IGuild guild) where T : IGuildRelation
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq(Fields.GuildId, guild.Id);

        var result = await collection.FindAsync(filter);

        return result.ToList();
    }
    
    public async Task<IReadOnlyCollection<T>> GetByGuildAsync<T>(string collectionName, IGuild guild, string name) where T : IGuildRelation, INamedEntity
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq(Fields.GuildId, guild.Id) & 
                     Builders<T>.Filter.Eq(Fields.Name, name);

        var result = await collection.FindAsync(filter);

        return result.ToList();
    }

    public async Task<IReadOnlyCollection<T>> GetByUserAsync<T>(string collectionName, IUser user) where T : IUserRelation
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq(Fields.UserId, user.Id);

        var result = await collection.FindAsync(filter);

        return result.ToList();
    }

    public async Task<IReadOnlyCollection<T>> GetByUserAsync<T>(string collectionName, IUser user, string name) where T : IUserRelation, INamedEntity
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq(Fields.UserId, user.Id) & 
                     Builders<T>.Filter.Eq(Fields.Name, name);

        var result = await collection.FindAsync(filter);

        return result.ToList();
    }

    public async Task<IReadOnlyCollection<T>> GetByGuildUserAsync<T>(string collectionName, IGuildUser guildUser) where T : IGuildUserRelation
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq(Fields.GuildId, guildUser.GuildId) &
                     Builders<T>.Filter.Eq(Fields.UserId, guildUser.Id);

        var result = await collection.FindAsync(filter);

        return result.ToList();
    }

    public async Task<IReadOnlyCollection<T>> GetByGuildUserAsync<T>(string collectionName, IGuildUser guildUser, string name) where T : IGuildUserRelation, INamedEntity
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq(Fields.GuildId, guildUser.GuildId) &
                     Builders<T>.Filter.Eq(Fields.UserId, guildUser.Id) &
                     Builders<T>.Filter.Eq(Fields.Name, name);

        var result = await collection.FindAsync(filter);

        return result.ToList();
    }

    public async Task<IReadOnlyCollection<T>> GetByChannelAsync<T>(string collectionName, IChannel channel) where T : IChannelRelation
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq(Fields.ChannelId, channel.Id);

        var result = await collection.FindAsync(filter);

        return result.ToList();
    }

    public async Task<IReadOnlyCollection<T>> GetByChannelAsync<T>(string collectionName, IChannel channel, string name) where T : IChannelRelation, INamedEntity
    {
        var collection = _service.Connect<T>(collectionName);

        var filter = Builders<T>.Filter.Eq(Fields.ChannelId, channel.Id) &
                     Builders<T>.Filter.Eq(Fields.Name, name);

        var result = await collection.FindAsync(filter);

        return result.ToList();
    }

    public async Task DeleteAsync<T>(string collectionName, T data) where T : IEntity
    {
        var collection = _service.Connect<T>(collectionName);

        await collection.DeleteOneAsync(x => x.Id == data.Id);
    }
}