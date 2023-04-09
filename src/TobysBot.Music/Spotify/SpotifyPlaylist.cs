using Discord;
using SpotifyAPI.Web;

namespace TobysBot.Music.Spotify;

public class SpotifyPlaylist : IPlaylist
{
    public SpotifyPlaylist(FullAlbum album, IUser requestedBy)
    {
        Title = album.Name;
        Url = $"https://open.spotify.com/album/{album.Id}";
        Tracks = album.Tracks.Items?.Select(x => new SpotifyTrack(x, requestedBy)) ?? Enumerable.Empty<ITrack>();
    }

    public SpotifyPlaylist(FullPlaylist playlist, IUser requestedBy)
    {
        Title = playlist.Name ?? "Unknown Playlist";
        Url = $"https://open.spotify.com/playlist/{playlist.Id}";
        Tracks = playlist.Tracks?.Items?
                     .Select(x => x.Track)
                     .OfType<FullTrack>()
                     .Select(x => new SpotifyTrack(x, requestedBy))
                 ?? Enumerable.Empty<ITrack>();
    }
    
    public string Title { get; }
    public string Url { get; }
    public IEnumerable<ITrack> Tracks { get; }
}