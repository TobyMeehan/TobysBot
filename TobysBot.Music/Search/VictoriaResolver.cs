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

    public async Task<ISearchResult> ResolveAsync(Uri uri)
    {
        var result = await _lavaNode.SearchAsync(SearchType.Direct, uri.AbsoluteUri);

        var track = result.Tracks.First();

        return result.Status switch
        {
            SearchStatus.TrackLoaded => new TrackResult(
                new Track(track.Title, track.Url, track.Url, track.Duration)),
            SearchStatus.PlaylistLoaded => new PlaylistResult(
                new Playlist(result.Tracks.Select(x => new Track(x.Title, x.Url, x.Duration)), result.Playlist.Name, uri.AbsoluteUri, result.Playlist.SelectedTrack)),
            SearchStatus.NoMatches => new NotFoundSearchResult(),
            _ => new LoadFailedSearchResult()
        };
    }

    public int Priority => 0;
}