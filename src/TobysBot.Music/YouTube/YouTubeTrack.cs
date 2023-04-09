using Discord;
using YoutubeExplode.Videos;

namespace TobysBot.Music.YouTube;

public class YouTubeTrack : ITrack
{
    public YouTubeTrack(IVideo video, IUser requestedBy)
    {
        Title = video.Title;
        Author = video.Author.ChannelTitle;
        Url = video.Url;
        Duration = video.Duration ?? throw new ArgumentNullException(nameof(video), "Video duration was null.");
        RequestedBy = requestedBy;
    }
    
    public string Title { get; }
    public string Author { get; }
    public string Url { get; }
    public TimeSpan Duration { get; }
    public IUser RequestedBy { get; }
}