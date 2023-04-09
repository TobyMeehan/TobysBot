using Discord;
using SpotifyAPI.Web;
using TobysBot.Music.Search.Result;

namespace TobysBot.Music.Spotify;

public class SpotifySearchResolver : ISearchResolver
{
    private readonly ISpotifyClient _spotify;

    public SpotifySearchResolver(ISpotifyClient spotify)
    {
        _spotify = spotify;
    }
    
    public bool CanResolve(Uri uri)
    {
        return uri.Host is "open.spotify.com";
    }

    public async Task<ISearchResult> ResolveAsync(Uri uri, IUser requestedBy)
    {
        return uri.Segments[1] switch
        {
            "track/" => await LoadTrackAsync(uri.Segments[2], requestedBy),
            "album/" => await LoadAlbumAsync(uri.Segments[2], requestedBy),
            "playlist/" => await LoadPlaylistAsync(uri.Segments[2], requestedBy),
            _ => new LoadFailedSearchResult("Could not parse Spotify url.")
        };
    }

    private async Task<ISearchResult> LoadTrackAsync(string id, IUser requestedBy)
    {
        FullTrack track;

        try
        {
            track = await _spotify.Tracks.Get(id);
        }
        catch
        {
            return new NotFoundSearchResult();
        }

        return new TrackResult(new SpotifyTrack(track, requestedBy));
    }

    private async Task<ISearchResult> LoadPlaylistAsync(string id, IUser requestedBy)
    {
        FullPlaylist playlist;

        try
        {
            playlist = await _spotify.Playlists.Get(id);
        }
        catch
        {
            return new NotFoundSearchResult();
        }

        if (playlist.Tracks?.Items is null)
        {
            return new LoadFailedSearchResult("Failed to load Spotify playlist.",
                new NullReferenceException("Playlist items was null."));
        }

        return new PlaylistResult(new SpotifyPlaylist(playlist, requestedBy));
    }

    private async Task<ISearchResult> LoadAlbumAsync(string id, IUser requestedBy)
    {
        FullAlbum album;

        try
        {
            album = await _spotify.Albums.Get(id);
        }
        catch
        {
            return new NotFoundSearchResult();
        }

        if (album.Tracks?.Items is null)
        {
            return new LoadFailedSearchResult("Failed to load Spotify album.",
                new NullReferenceException("Album items was null."));
        }

        return new PlaylistResult(new SpotifyPlaylist(album, requestedBy));
    }

    public int Priority => 200;
}