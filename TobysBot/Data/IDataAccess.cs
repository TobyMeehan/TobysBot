namespace TobysBot.Data;

public interface IDataAccess
{
    Task<T> SaveAsync<T>(T data) where T : IEntity;

    Task<IReadOnlyCollection<T>> GetAsync<T>() where T : IEntity;

    Task<T> GetAsync<T>(string id) where T : IEntity;

    Task<T> GetAsync<T>(ulong id) where T : IDiscordEntity;

    Task<T> GetByNameAsync<T>(string name) where T : INamedEntity;

    Task<T> UpdateAsync<T>(T data) where T : IEntity;

    Task DeleteAsync<T>(T data) where T : IEntity;
}