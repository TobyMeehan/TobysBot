namespace TobysBot.Music.Search.Result;

public class PlaylistResult : ISearchResult
{
    public PlaylistResult(IPlaylist playlist)
    {
        Playlist = playlist;
    }

    public PlaylistResult(IEnumerable<TrackResult> tracks, string title, string url)
    {
        Playlist = new Playlist(
            from track in tracks select track.Track, 
            title, url, 0);
    }

    public IPlaylist Playlist { get; }
}