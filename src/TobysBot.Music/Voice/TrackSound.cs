using TobysBot.Voice;

namespace TobysBot.Music.Voice;

public class TrackSound : ISound
{
    public TrackSound(ITrack track)
    {
        Title = track.Title;
        Url = track.Url;
        Duration = track.Duration;
    }
    
    public string Title { get; }
    public string Url { get; }
    public TimeSpan Duration { get; }
}