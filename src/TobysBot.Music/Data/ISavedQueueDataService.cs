using Discord;

namespace TobysBot.Music.Data;

public interface ISavedQueueDataService
{
    Task<IReadOnlyCollection<ISavedQueue>> ListSavedQueuesAsync(IUser user);
    Task<ISavedQueue> GetSavedQueueAsync(string id, IUser requestedBy);
    Task<ISavedQueue?> GetSavedQueueAsync(IUser user, string name, IUser requestedBy);
    Task CreateSavedQueueAsync(string name, IUser user, IQueue queue);
    Task DeleteSavedQueueAsync(IUser user, string name);

    Uri GetShareUri(ISavedQueue savedQueue);
    string ParseShareUri(Uri uri);
}