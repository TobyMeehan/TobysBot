using YoutubeExplode.Videos;

namespace TobysBot.Music.Search;

public class YouTubeTrack : ITrack
{
    public YouTubeTrack(IVideo video)
    {
        Title = video.Title;
        Url = AudioUrl = video.Url;
        Duration = video.Duration ?? throw new ArgumentNullException(nameof(video.Duration));
    }
    
    public string Title { get; }
    public string Url { get; }
    public string AudioUrl { get; }
    public TimeSpan Duration { get; }
}