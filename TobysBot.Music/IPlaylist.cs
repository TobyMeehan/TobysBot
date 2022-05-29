namespace TobysBot.Music;

public interface IPlaylist
{
    string Title { get; }
    string Url { get; }
    
    IReadOnlyCollection<ITrack> Tracks { get; }
}