namespace TobysBot.Data;

public interface IDataAccess
{
    Task<T> SaveAsync<T>(string collectionName, T data) where T : IEntity;

    Task<IReadOnlyCollection<T>> GetAsync<T>(string collectionName) where T : IEntity;

    Task<T> GetAsync<T>(string collectionName, string id) where T : IEntity;

    Task<T> GetAsync<T>(string collectionName, ulong id) where T : IDiscordEntity;

    Task<T> GetByNameAsync<T>(string collectionName, string name) where T : INamedEntity;

    Task<T> UpdateAsync<T>(string collectionName, T data) where T : IEntity;

    Task DeleteAsync<T>(string collectionName, T data) where T : IEntity;
}