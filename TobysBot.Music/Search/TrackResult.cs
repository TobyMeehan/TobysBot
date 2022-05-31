namespace TobysBot.Music.Search;

public class TrackResult : ITrack, ISearchResult
{
    public TrackResult(string title, string url, string audioUrl, TimeSpan duration)
    {
        Title = title;
        Url = url;
        AudioUrl = audioUrl;
        Duration = duration;
    }

    public string Title { get; }
    public string Url { get; }
    public string AudioUrl { get; }
    public TimeSpan Duration { get; }
}