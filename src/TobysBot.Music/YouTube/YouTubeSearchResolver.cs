using System.Web;
using Discord;
using TobysBot.Music.Search.Result;
using YoutubeExplode;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Videos;

namespace TobysBot.Music.YouTube;

public class YouTubeSearchResolver : ISearchResolver
{
    private readonly YoutubeClient _youtube;

    public YouTubeSearchResolver(YoutubeClient youtube)
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
            "watch" when query["v"] is not null => await LoadTrackAsync(query["v"]!, requestedBy),
            "playlist" when query["list"] is not null => await LoadPlaylistAsync(query["list"]!, requestedBy),
            "shorts/" => await LoadTrackAsync(uri.Segments.Last(), requestedBy),
            _ => new LoadFailedSearchResult("Could not parse YouTube url.")
        };
    }

    private async Task<ISearchResult> LoadTrackAsync(string id, IUser requestedBy)
    {
        IVideo video;

        try
        {
            video = await _youtube.Videos.GetAsync(id);
        }
        catch (ArgumentException)
        {
            return new LoadFailedSearchResult("Invalid video URL.");
        }
        catch (VideoUnavailableException)
        {
            return new NotFoundSearchResult();
        }
        catch
        {
            return new LoadFailedSearchResult();
        }

        return new TrackResult(new YouTubeTrack(video, requestedBy));
    }

    private async Task<ISearchResult> LoadPlaylistAsync(string id, IUser requestedBy)
    {
        var playlist = await _youtube.Playlists.GetAsync(id);
        var videos = _youtube.Playlists.GetVideosAsync(playlist.Id).ToEnumerable();

        return new PlaylistResult(new YouTubePlaylist(playlist, videos, requestedBy));
    }

    public int Priority => 100;
}