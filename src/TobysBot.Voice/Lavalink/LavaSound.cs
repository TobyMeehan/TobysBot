using Victoria;

namespace TobysBot.Voice.Lavalink;

public class LavaSound : ISound
{
    public LavaSound(LavaTrack track)
    {
        Title = track.Title;
        Url = track.Url;
        Duration = track.Duration;
    }
    
    public string Title { get; }
    public string Url { get; }
    public TimeSpan Duration { get; }
}