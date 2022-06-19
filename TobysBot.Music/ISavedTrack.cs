namespace TobysBot.Music;

public interface ISavedTrack
{
    string Title { get; }
    string Url { get; }
    TimeSpan Duration { get; }
}