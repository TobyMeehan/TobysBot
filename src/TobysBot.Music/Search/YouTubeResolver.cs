using System.Web;
using TobysBot.Music.Search.Result;
using YoutubeExplode;

namespace TobysBot.Music.Search;

public class YouTubeResolver : ISearchResolver
{
    private readonly YoutubeClient _youtube;

    public YouTubeResolver(YoutubeClient youtube)
    {
        _youtube = youtube;
    }

    public bool CanResolve(Uri uri)
    {
        return uri.Host is "youtube.com" or "www.youtube.com" or "music.youtube.com";
    }

    public async Task<ISearchResult> ResolveAsync(Uri uri)
    {
        var query = HttpUtility.ParseQueryString(uri.Query);

        return uri.Segments[1] switch
        {
            "watch" when query["v"] is not null => await LoadYouTubeTrackAsync(query["v"]!),
            "playlist" when query["list"] is not null => await LoadYouTubePlaylistAsync(query["list"]!),
            "shorts/" => await LoadYouTubeTrackAsync(uri.Segments.Last()),
            _ => new LoadFailedSearchResult("Could not parse YouTube url.")
        };
    }

    private async Task<ISearchResult> LoadYouTubePlaylistAsync(string id)
    {
        var playlist = await _youtube.Playlists.GetAsync(id);
        var videos = _youtube.Playlists.GetVideosAsync(playlist.Id).ToEnumerable();

        return new PlaylistResult(
            new YouTubePlaylist(playlist, videos));
    }

    private async Task<ISearchResult> LoadYouTubeTrackAsync(string id)
    {
        var video = await _youtube.Videos.GetAsync(id);

        return new TrackResult(
            new YouTubeTrack(video));
    }

    public int Priority => 100;
}