using Discord;

namespace TobysBot.Music;

public interface ITrack
{
    string Title { get; }
    string Author { get; }
    string Url { get; }
    string AudioUrl { get; }
    TimeSpan Duration { get; }
    IUser RequestedBy { get; }
}