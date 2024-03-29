using Discord;

namespace TobysBot.Music.MemoryQueue;

public class ActiveTrack : IActiveTrack
{
    public ActiveTrack(ITrack innerTrack, TimeSpan position, ActiveTrackStatus status)
    {
        Position = position;
        Status = status;
        InnerTrack = innerTrack;
    }

    public string Title => InnerTrack.Title;
    public string Author => InnerTrack.Author;
    public string Url => InnerTrack.Url;
    public string AudioUrl => InnerTrack.AudioUrl;
    public TimeSpan Duration => InnerTrack.Duration;
    public IUser RequestedBy => InnerTrack.RequestedBy;
    public TimeSpan Position { get; }
    
    public ITrack InnerTrack { get; }
    
    public ActiveTrackStatus Status { get; }
}