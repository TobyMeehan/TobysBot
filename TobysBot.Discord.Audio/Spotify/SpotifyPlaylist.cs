using System.Collections;
using System.Collections.Generic;
using SpotifyAPI.Web;

namespace TobysBot.Discord.Audio.Spotify;

public class SpotifyPlaylist : IPlaylist
{
    private readonly IEnumerable<SpotifyTrack> _tracks;

    public SpotifyPlaylist(FullAlbum album, IEnumerable<SpotifyTrack> tracks)
    {
        _tracks = tracks;
        Url = $"https://open.spotify.com/album/{album.Id}";
        Title = album.Name;
    }

    public SpotifyPlaylist(FullPlaylist playlist, IEnumerable<SpotifyTrack> tracks)
    {
        _tracks = tracks;
        Url = $"https://open.spotify.com/playlist/{playlist.Id}";
        Title = playlist.Name;
    }
    
    public string Url { get; }
    public string SourceUrl => Url;
    public string Title { get; }
    
    public IEnumerator<ITrack> GetEnumerator()
    {
        return _tracks.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}