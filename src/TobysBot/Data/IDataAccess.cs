using Discord;

namespace TobysBot.Data;

/// <summary>
/// Abstraction for database connection.
/// </summary>
public interface IDataAccess
{
    /// <summary>
    /// Saves the specified <see cref="IEntity"/> to the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> SaveAsync<T>(string collectionName, T data) where T : IEntity;
    
    /// <summary>
    /// Saves the specified <see cref="INamedEntity"/> to the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> SaveByNameAsync<T>(string collectionName, T data) where T : INamedEntity;
    
    /// <summary>
    /// Saves the specified <see cref="INamedEntity"/> to the specified <see cref="collectionName"/> with a user relation.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> SaveByUserAsync<T>(string collectionName, T data) where T : IUserRelation, INamedEntity;
    
    /// <summary>
    /// Saves the specified <see cref="INamedEntity"/> to the specified <see cref="collectionName"/> with a guild relation.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> SaveByGuildAsync<T>(string collectionName, T data) where T : IGuildRelation, INamedEntity;

    /// <summary>
    /// Saves the specified <see cref="INamedEntity"/> to the specified <see cref="collectionName"/> with a guild user relation.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> SaveByGuildUserAsync<T>(string collectionName, T data)
        where T : IGuildUserRelation, INamedEntity;

    /// <summary>
    /// Saves the specified <see cref="INamedEntity"/> to the specified <see cref="collectionName"/> with a channel relation.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> SaveByChannelAsync<T>(string collectionName, T data)
        where T : IChannelRelation, INamedEntity;

    /// <summary>
    /// Gets all entities from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetAsync<T>(string collectionName) where T : IEntity;

    /// <summary>
    /// Gets the entity with the specified <see cref="id"/> from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T?> GetAsync<T>(string collectionName, string id) where T : IEntity;

    /// <summary>
    /// Gets the entity with the specified Discord ID from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T?> GetAsync<T>(string collectionName, ulong id) where T : IDiscordEntity;

    /// <summary>
    /// Gets all entities with the specified <see cref="name"/> from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetByNameAsync<T>(string collectionName, string name) where T : INamedEntity;
    
    /// <summary>
    /// Gets all entities with the specified guild from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="guild"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetByGuildAsync<T>(string collectionName, IGuild guild) where T : IGuildRelation;

    /// <summary>
    /// Gets all entities with the specified name and guild from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="guild"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetByGuildAsync<T>(string collectionName, IGuild guild, string name)
        where T : IGuildRelation, INamedEntity;

    /// <summary>
    /// Gets all entities with the specified user from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="user"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetByUserAsync<T>(string collectionName, IUser user) where T : IUserRelation;

    /// <summary>
    /// Gets all entities with the specified name and user from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetByUserAsync<T>(string collectionName, IUser user, string name)
        where T : IUserRelation, INamedEntity;

    /// <summary>
    /// Gets all entities with the specified guild user from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="guildUser"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetByGuildUserAsync<T>(string collectionName, IGuildUser guildUser)
        where T : IGuildUserRelation;

    /// <summary>
    /// Gets all entities with the specified name and guild user from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="guildUser"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetByGuildUserAsync<T>(string collectionName, IGuildUser guildUser, string name)
        where T : IGuildUserRelation, INamedEntity;

    /// <summary>
    /// Gets all entities with the specified channel from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="channel"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetByChannelAsync<T>(string collectionName, IChannel channel) where T : IChannelRelation;

    /// <summary>
    /// Gets all entities with the specified name and channel from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="channel"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetByChannelAsync<T>(string collectionName, IChannel channel, string name)
        where T : IChannelRelation, INamedEntity;

    /// <summary>
    /// Deletes the specified entity from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteAsync<T>(string collectionName, T data) where T : IEntity;

    /// <summary>
    /// Deletes the entity with the specified <see cref="id"/> from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteAsync<T>(string collectionName, string id) where T : IEntity;

    /// <summary>
    /// Deletes all entities in the specified guild from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="guild"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteAsync<T>(string collectionName, IGuild guild) where T : IGuildRelation;

    /// <summary>
    /// Deletes all entities with the specified name in the specified guild from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="guild"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteAsync<T>(string collectionName, IGuild guild, string name) where T : IGuildRelation, INamedEntity;

    /// <summary>
    /// Deletes all entities with the specified user from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="user"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteAsync<T>(string collectionName, IUser user) where T : IUserRelation;

    /// <summary>
    /// Deletes all entities with the specified name and user from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteAsync<T>(string collectionName, IUser user, string name) where T : IUserRelation, INamedEntity;

    /// <summary>
    /// Deletes all entities with the specified guild user from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="guildUser"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteAsync<T>(string collectionName, IGuildUser guildUser) where T : IGuildUserRelation;

    /// <summary>
    /// Deletes all entities with the specified name and guild user from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="guildUser"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteAsync<T>(string collectionName, IGuildUser guildUser, string name)
        where T : IGuildUserRelation, INamedEntity;

    /// <summary>
    /// Deletes all entities with the specified channel from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="channel"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteAsync<T>(string collectionName, IChannel channel) where T : IChannelRelation;

    /// <summary>
    /// Deletes all entities with the specified name and channel from the specified <see cref="collectionName"/>.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="channel"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task DeleteAsync<T>(string collectionName, IChannel channel, string name) where T : IChannelRelation, INamedEntity;
}