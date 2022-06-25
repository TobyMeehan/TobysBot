using Discord;
using Microsoft.Extensions.Options;
using TobysBot.Data;
using TobysBot.Hosting;
using TobysBot.Music.Configuration;

namespace TobysBot.Music.Data;

public class SavedQueueDataService : ISavedQueueDataService
{
    private readonly IDataAccess _data;
    private readonly IHostingService _server;
    private readonly MusicDataOptions _options;

    public SavedQueueDataService(IDataAccess data, IHostingService server, IOptions<MusicOptions> options)
    {
        _data = data;
        _server = server;

        _options = options.Value.Data ?? throw new NullReferenceException("No music data options specified.");
    }

    public async Task<IReadOnlyCollection<ISavedQueue>> ListSavedQueuesAsync(IUser user)
    {
        if (_options.SavedQueueCollection is null)
        {
            throw new NullReferenceException("Saved queue collection name not specified.");
        }
        
        return await _data.GetByUserAsync<SavedQueue>(_options.SavedQueueCollection, user);
    }

    public async Task<ISavedQueue> GetSavedQueueAsync(string id, IUser requestedBy)
    {
        if (_options.SavedQueueCollection is null)
        {
            throw new NullReferenceException("Saved queue collection name not specified.");
        }
        
        var queue = await _data.GetAsync<SavedQueue>(_options.SavedQueueCollection, id);

        foreach (var track in queue.Tracks)
        {
            track.RequestedBy = requestedBy;
        }

        return queue;
    }

    public async Task<ISavedQueue?> GetSavedQueueAsync(IUser user, string name, IUser requestedBy)
    {
        if (_options.SavedQueueCollection is null)
        {
            throw new NullReferenceException("Saved queue collection name not specified.");
        }
        
        IReadOnlyCollection<SavedQueue?> result = await _data.GetByUserAsync<SavedQueue>(_options.SavedQueueCollection, user, name);

        var queue = result.FirstOrDefault();

        if (queue is null)
        {
            return queue;
        }
        
        foreach (var track in queue.Tracks)
        {
            track.RequestedBy = requestedBy;
        }

        return queue;
    }

    public async Task CreateSavedQueueAsync(string name, IUser user, IQueue queue)
    {
        if (_options.SavedQueueCollection is null)
        {
            throw new NullReferenceException("Saved queue collection name not specified.");
        }
        
        await _data.SaveByUserAsync(_options.SavedQueueCollection, new SavedQueue(name, user, queue));
    }

    public async Task DeleteSavedQueueAsync(IUser user, string name)
    {
        if (_options.SavedQueueCollection is null)
        {
            throw new NullReferenceException("Saved queue collection name not specified.");
        }
        
        await _data.DeleteAsync<SavedQueue>(_options.SavedQueueCollection, user, name);
    }

    public Uri GetShareUri(ISavedQueue savedQueue)
    {
        return new Uri($"https://{_server.ServerName}/savedqueues/{savedQueue.Id}");
    }

    public string ParseShareUri(Uri uri)
    {
        if (uri.Authority != _server.ServerName ||
            uri.Segments[1] != "savedqueues/")
        {
            throw new ArgumentException("Invalid hostname or path.", nameof(uri));
        }

        return uri.Segments[2];
    }
}