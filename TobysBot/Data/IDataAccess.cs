using Discord;

namespace TobysBot.Data;

public interface IDataAccess
{
    Task<T> SaveAsync<T>(string collectionName, T data) where T : IEntity;

    Task<IReadOnlyCollection<T>> GetAsync<T>(string collectionName) where T : IEntity;

    Task<T> GetAsync<T>(string collectionName, string id) where T : IEntity;

    Task<T> GetAsync<T>(string collectionName, ulong id) where T : IDiscordEntity;

    Task<IReadOnlyCollection<T>> GetByNameAsync<T>(string collectionName, string name) where T : INamedEntity;
    
    Task<IReadOnlyCollection<T>> GetByGuildAsync<T>(string collectionName, IGuild guild) where T : IGuildRelation;

    Task<IReadOnlyCollection<T>> GetByGuildAsync<T>(string collectionName, IGuild guild, string name)
        where T : IGuildRelation, INamedEntity;

    Task<IReadOnlyCollection<T>> GetByUserAsync<T>(string collectionName, IUser user) where T : IUserRelation;

    Task<IReadOnlyCollection<T>> GetByUserAsync<T>(string collectionName, IUser user, string name)
        where T : IUserRelation, INamedEntity;

    Task<IReadOnlyCollection<T>> GetByGuildUserAsync<T>(string collectionName, IGuildUser guildUser)
        where T : IGuildUserRelation;

    Task<IReadOnlyCollection<T>> GetByGuildUserAsync<T>(string collectionName, IGuildUser guildUser, string name)
        where T : IGuildUserRelation, INamedEntity;

    Task<IReadOnlyCollection<T>> GetByChannelAsync<T>(string collectionName, IChannel channel) where T : IChannelRelation;

    Task<IReadOnlyCollection<T>> GetByChannelAsync<T>(string collectionName, IChannel channel, string name)
        where T : IChannelRelation, INamedEntity;

    Task DeleteAsync<T>(string collectionName, T data) where T : IEntity;
}