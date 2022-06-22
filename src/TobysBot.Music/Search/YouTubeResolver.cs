using System.Web;
using Discord;
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

    public async Task<ISearchResult> ResolveAsync(Uri uri, IUser requestedBy)
    {
        var query = HttpUtility.ParseQueryString(uri.Query);

        return uri.Segments[1] switch
        {
            "watch" when query["v"] is not null => await LoadYouTubeTrackAsync(query["v"]!, requestedBy),
            "playlist" when query["list"] is not null => await LoadYouTubePlaylistAsync(query["list"]!, requestedBy),
            "shorts/" => await LoadYouTubeTrackAsync(uri.Segments.Last(), requestedBy),
            _ => new LoadFailedSearchResult("Could not parse YouTube url.")
        };
    }

    private async Task<ISearchResult> LoadYouTubePlaylistAsync(string id, IUser requestedBy)
    {
        var playlist = await _youtube.Playlists.GetAsync(id);
        var videos = _youtube.Playlists.GetVideosAsync(playlist.Id).ToEnumerable();

        return new PlaylistResult(
            new YouTubePlaylist(playlist, videos, requestedBy));
    }

    private async Task<ISearchResult> LoadYouTubeTrackAsync(string id, IUser requestedBy)
    {
        var video = await _youtube.Videos.GetAsync(id);

        return new TrackResult(
            new YouTubeTrack(video, requestedBy));
    }

    public int Priority => 100;
}