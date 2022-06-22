using Discord;
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

    public async Task<ISearchResult> ResolveAsync(Uri uri, IUser requestedBy)
    {
        return uri.Segments[1] switch
        {
            "track/" => await LoadSpotifyTrackAsync(uri.Segments[2], requestedBy),
            "album/" => await LoadSpotifyAlbumAsync(uri.Segments[2], requestedBy),
            "playlist/" => await LoadSpotifyPlaylistAsync(uri.Segments[2], requestedBy),
            _ => throw new Exception("Could not parse Spotify url.")
        };
    }

    private async Task<ISearchResult> LoadSpotifyPlaylistAsync(string id, IUser requestedBy)
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
            if (item.Track is not FullTrack fullTrack)
            {
                continue;
            }
            
            var track = await LoadSpotifyTrackAsync(fullTrack.Id, requestedBy);

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

    private async Task<ISearchResult> LoadSpotifyAlbumAsync(string id, IUser requestedBy)
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
            var track = await LoadSpotifyTrackAsync(item.Id, requestedBy);

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

    private async Task<ISearchResult> LoadSpotifyTrackAsync(string id, IUser requestedBy)
    {
        var track = await _spotify.Tracks.Get(id);

        var search = _youtube.Search.GetVideosAsync($"{track.Artists[0].Name} {track.Name}");

        await foreach (var video in search)
        {
            if (!video.Duration.HasValue)
            {
                continue;
            }

            if ((video.Duration.Value - TimeSpan.FromMilliseconds(track.DurationMs)).Duration() > TimeSpan.FromSeconds(10))
            {
                continue;
            }
            
            return new TrackResult(
                new SpotifyTrack(track, video.Url, video.Duration.Value, requestedBy));
        }

        return new LoadFailedSearchResult($"Failed to load Spotify track {track.Name}.");

        
    }

    public int Priority => 200;
}