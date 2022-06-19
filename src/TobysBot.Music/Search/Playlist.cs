namespace TobysBot.Music.Search;

public class Playlist : IPlaylist
{
    public Playlist(IEnumerable<ITrack> tracks, string title, string url, int startPos)
    {
        Tracks = tracks.Skip(startPos);
        
        Title = title;
        Url = url;
    }

    public string Title { get; }
    public string Url { get; }
    public IEnumerable<ITrack> Tracks { get; }
}