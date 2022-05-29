namespace TobysBot.Music;

public interface ITrack
{
    string Title { get; }
    string Url { get; }
    string AudioUrl { get; }
    TimeSpan Duration { get; }
}