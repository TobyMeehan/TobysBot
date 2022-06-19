﻿using Discord;
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
        _options = options.Value.Data;
    }

    public async Task<IReadOnlyCollection<ISavedQueue>> ListSavedQueuesAsync(IUser user)
    {
        return await _data.GetByUserAsync<SavedQueue>(_options.SavedQueueCollection, user);
    }

    public async Task<ISavedQueue> GetSavedQueueAsync(string id)
    {
        return await _data.GetAsync<SavedQueue>(_options.SavedQueueCollection, id);
    }

    public async Task<ISavedQueue> GetSavedQueueAsync(IUser user, string name)
    {
        var result = await _data.GetByUserAsync<SavedQueue>(_options.SavedQueueCollection, user, name);

        return result.FirstOrDefault();
    }

    public async Task CreateSavedQueueAsync(string name, IUser user, IQueue queue)
    {
        await _data.SaveByUserAsync(_options.SavedQueueCollection, new SavedQueue(name, user, queue));
    }

    public async Task DeleteSavedQueueAsync(IUser user, string name)
    {
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