namespace TobysBot.Music;

public interface IPlaylist
{
    string Title { get; }
    string Url { get; }
    
    IEnumerable<ITrack> Tracks { get; }
}