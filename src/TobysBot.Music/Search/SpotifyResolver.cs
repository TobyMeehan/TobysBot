using SpotifyAPI.Web;
using TobysBot.Music.Search.Result;
using YoutubeExplode;

namespace TobysBot.Music.Search;

public class SpotifyResolver : ISearchResolver
{
    private readonly ISpotifyClient _spotify;
    private readonly YoutubeClient _youtube;

    public SpotifyResolver(ISpotifyClient spotify, YoutubeClient youtube)
    {
        _spotify = spotify;
        _youtube = youtube;
    }
    
    public bool CanResolve(Uri uri)
    {
        return uri.Host is "open.spotify.com";
    }

    public async Task<ISearchResult> ResolveAsync(Uri uri)
    {
        return uri.Segments[1] switch
        {
            "track/" => await LoadSpotifyTrackAsync(uri.Segments[2]),
            "album/" => await LoadSpotifyAlbumAsync(uri.Segments[2]),
            "playlist/" => await LoadSpotifyPlaylistAsync(uri.Segments[2]),
            _ => throw new Exception("Could not parse Spotify url.")
        };
    }

    private async Task<ISearchResult> LoadSpotifyPlaylistAsync(string id)
    {
        var playlist = await _spotify.Playlists.Get(id);

        if (playlist.Tracks?.Items is null)
        {
            return new LoadFailedSearchResult("Failed to load Spotify playlist.",
                new NullReferenceException("Playlist items was null."));
        }

        var tracks = new List<ITrack>();

        foreach (var item in playlist.Tracks.Items.Where(item => item.Track.Type is not ItemType.Episode))
        {
            var track = await LoadSpotifyTrackAsync((item.Track as FullTrack)?.Id);

            if (track is TrackResult spotifyTrack)
            {
                tracks.Add(spotifyTrack.Track);
            }
        }

        if (!tracks.Any())
        {
            return new LoadFailedSearchResult("Failed to load Spotify playlist.",
                new Exception("Playlist's tracks was empty."));
        }

        return new PlaylistResult(
            new SpotifyPlaylist(playlist, tracks));
    }

    private async Task<ISearchResult> LoadSpotifyAlbumAsync(string id)
    {
        var album = await _spotify.Albums.Get(id);

        if (album.Tracks?.Items is null)
        {
            return new LoadFailedSearchResult("Failed to load Spotify album.",
                new NullReferenceException("Album items was null."));
        }

        var tracks = new List<ITrack>();

        foreach (var item in album.Tracks.Items)
        {
            var track = await LoadSpotifyTrackAsync(item.Id);

            if (track is TrackResult spotifyTrack)
            {
                tracks.Add(spotifyTrack.Track);
            }
        }

        if (!tracks.Any())
        {
            return new LoadFailedSearchResult("Failed to load Spotify album.",
                new Exception("Album's tracks was empty."));
        }

        return new PlaylistResult(
            new SpotifyPlaylist(album, tracks));
    }

    private async Task<ISearchResult> LoadSpotifyTrackAsync(string id)
    {
        var track = await _spotify.Tracks.Get(id);

        var search = _youtube.Search.GetVideosAsync($"{track.Artists[0].Name} {track.Name}");

        var video = await search.FirstAsync();

        if (!video.Duration.HasValue)
        {
            return new LoadFailedSearchResult($"Failed to load Spotify track {track.Name}.",
                new NullReferenceException("YouTube video duration was null."));
        }

        return new TrackResult(
            new SpotifyTrack(track, video.Url, video.Duration.Value));
    }

    public int Priority => 200;
}