using Discord;
using TobysBot.Music.Exceptions;
using TobysBot.Music.Search.Result;
using YoutubeExplode;

namespace TobysBot.Music.Search;

public class SearchService : ISearchService
{
    private readonly IEnumerable<ISearchResolver> _resolvers;
    private readonly YoutubeClient _youtube;

    public SearchService(IEnumerable<ISearchResolver> resolvers, YoutubeClient youtube)
    {
        _resolvers = resolvers.OrderByDescending(x => x.Priority);
        _youtube = youtube;
    }

    private async Task<ISearchResult> ResolveAsync(Uri uri)
    {
        foreach (var resolver in _resolvers)
        {
            if (resolver.CanResolve(uri))
            {
                return await resolver.ResolveAsync(uri);
            }
        }

        return new LoadFailedSearchResult("Could not resolve url.");
    }

    public async Task<ISearchResult> SearchAsync(string query)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }
        
        if (Uri.TryCreate(query, UriKind.Absolute, out var uri))
        {
            return await ResolveAsync(uri);
        }

        var result = _youtube.Search.GetVideosAsync(query);

        var video = await result.FirstOrDefaultAsync();

        if (video is null)
        {
            throw new NoResultsException(query);
        }

        return new TrackResult(
            new YouTubeTrack(video));
    }

    public async Task<ISearchResult> LoadAttachmentsAsync(IMessage message)
    {
        throw new NotImplementedException();
    }
}