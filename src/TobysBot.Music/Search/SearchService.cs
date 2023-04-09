using Discord;
using TobysBot.Music.Exceptions;
using TobysBot.Music.Search.Result;
using TobysBot.Music.YouTube;
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

    private async Task<ISearchResult> ResolveAsync(Uri uri, IUser requestedBy)
    {
        foreach (var resolver in _resolvers)
        {
            if (resolver.CanResolve(uri))
            {
                return await resolver.ResolveAsync(uri, requestedBy);
            }
        }

        return new LoadFailedSearchResult("Could not resolve url.");
    }

    public async Task<ISearchResult> SearchAsync(string query, IUser requestedBy)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }
        
        if (Uri.TryCreate(query, UriKind.Absolute, out var uri))
        {
            return await ResolveAsync(uri, requestedBy);
        }

        var result = _youtube.Search.GetVideosAsync(query);

        var video = await result.FirstOrDefaultAsync();

        if (video is null)
        {
            throw new NoResultsException(query);
        }

        return new TrackResult(
            new YouTubeTrack(video, requestedBy));
    }

    public async Task<ISearchResult> LoadAttachmentsAsync(IMessage message, IUser requestedBy)
    {
        var tracks = new List<TrackResult>();

        foreach (var attachment in message.Attachments)
        {
            var result = await ResolveAsync(new Uri(attachment.Url), requestedBy);

            if (result is TrackResult track)
            {
                tracks.Add(track);
            }
        }

        if (!tracks.Any())
        {
            return new NotFoundSearchResult();
        }

        if (tracks.Count is 1)
        {
            return tracks[0];
        }

        return new PlaylistResult(tracks, "Attachments", message.GetJumpUrl());
    }
}