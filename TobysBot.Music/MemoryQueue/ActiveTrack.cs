namespace TobysBot.Music.MemoryQueue;

public class ActiveTrack : IActiveTrack
{
    private readonly ITrack _innerTrack;

    public ActiveTrack(ITrack innerTrack, TimeSpan position, ActiveTrackStatus status)
    {
        Position = position;
        Status = status;
        _innerTrack = innerTrack;
    }

    public string Title => _innerTrack.Title;
    public string Url => _innerTrack.Url;
    public string AudioUrl => _innerTrack.AudioUrl;
    public TimeSpan Duration => _innerTrack.Duration;
    public TimeSpan Position { get; }
    
    public ActiveTrackStatus Status { get; }
}