using Discord;
using TobysBot.Hosting;
using TobysBot.Music.Data;
using TobysBot.Music.Search.Result;

namespace TobysBot.Music.Search;

public class SavedQueueResolver : ISearchResolver
{
    private readonly IHostingService _server;
    private readonly ISavedQueueDataService _data;

    public SavedQueueResolver(IHostingService server, ISavedQueueDataService data)
    {
        _server = server;
        _data = data;
    }
    
    public bool CanResolve(Uri uri)
    {
        return uri.Authority == _server.ServerName &&
               uri.Segments[1] == "savedqueues/" &&
               uri.Segments.Length >= 3;
    }

    public async Task<ISearchResult> ResolveAsync(Uri uri, IUser requestedBy)
    {
        string id = _data.ParseShareUri(uri);
        var savedQueue = await _data.GetSavedQueueAsync(id, requestedBy);

        if (savedQueue is null)
        {
            return new NotFoundSearchResult();
        }

        return new SavedQueueResult(savedQueue);
    }

    public int Priority => 50;
}