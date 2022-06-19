using SpotifyAPI.Web;

namespace TobysBot.Music.Search;

public class SpotifyPlaylist : IPlaylist
{
    public SpotifyPlaylist(FullAlbum album, IEnumerable<ITrack> tracks)
    {
        Tracks = tracks;
        Url = $"https://open.spotify.com/album/{album.Id}";
        Title = album.Name;
    }

    public SpotifyPlaylist(FullPlaylist playlist, IEnumerable<ITrack> tracks)
    {
        Tracks = tracks;
        Url = $"https://open.spotify.com/playlist/{playlist.Id}";
        Title = playlist.Name;
    }
    
    public string Title { get; }
    public string Url { get; }
    public IEnumerable<ITrack> Tracks { get; }
}