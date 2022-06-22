using Discord;
using TobysBot.Music.Search.Result;
using Victoria;
using Victoria.Responses.Search;

namespace TobysBot.Music.Search;

public class VictoriaResolver : ISearchResolver
{
    private readonly LavaNode _lavaNode;

    public VictoriaResolver(LavaNode lavaNode)
    {
        _lavaNode = lavaNode;
    }
    
    public bool CanResolve(Uri uri)
    {
        return true;
    }

    public async Task<ISearchResult> ResolveAsync(Uri uri, IUser requestedBy)
    {
        var result = await _lavaNode.SearchAsync(SearchType.Direct, uri.AbsoluteUri);

        var track = result.Tracks.FirstOrDefault();

        return result.Status switch
        {
            SearchStatus.TrackLoaded when track is not null => new TrackResult(
                new Track(track.Title, track.Author, track.Url, track.Duration, requestedBy)),
            SearchStatus.PlaylistLoaded when result.Tracks.Any() => new PlaylistResult(
                new Playlist(result.Tracks.Select(x => new Track(x.Title, x.Author, x.Url, x.Duration, requestedBy)), result.Playlist.Name, uri.AbsoluteUri, result.Playlist.SelectedTrack)),
            SearchStatus.NoMatches => new NotFoundSearchResult(),
            _ => new LoadFailedSearchResult("No audio found at that url.")
        };
    }

    public int Priority => 0;
}