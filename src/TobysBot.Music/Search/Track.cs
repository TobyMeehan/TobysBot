using Discord;

namespace TobysBot.Music.Search;

public class Track : ITrack
{
    public Track(string title, string author, string url, string audioUrl, TimeSpan duration, IUser requestedBy)
    {
        Title = title;
        Url = url;
        AudioUrl = audioUrl;
        Duration = duration;
        RequestedBy = requestedBy;
        Author = author;
    }

    public Track(string title, string author, string url, TimeSpan duration, IUser requestedBy)
    {
        Title = title;
        Url = url;
        AudioUrl = url;
        Duration = duration;
        RequestedBy = requestedBy;
        Author = author;
    }

    public string Title { get; }
    public string Author { get; }
    public string Url { get; }
    public string AudioUrl { get; }
    public TimeSpan Duration { get; }
    public IUser RequestedBy { get; }
}