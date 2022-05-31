namespace TobysBot.Music.Search;

public class Track : ITrack
{
    public Track(string title, string url, string audioUrl, TimeSpan duration)
    {
        Title = title;
        Url = url;
        AudioUrl = audioUrl;
        Duration = duration;
    }

    public Track(string title, string url, TimeSpan duration)
    {
        Title = title;
        Url = url;
        AudioUrl = url;
        Duration = duration;
    }

    public string Title { get; }
    public string Url { get; }
    public string AudioUrl { get; }
    public TimeSpan Duration { get; }
}