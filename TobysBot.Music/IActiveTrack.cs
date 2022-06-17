namespace TobysBot.Music;

public interface IActiveTrack : ITrack
{
    TimeSpan Position { get; }
    
    ActiveTrackStatus Status { get; }
    
    ITrack InnerTrack { get; }
}

public enum ActiveTrackStatus
{
    Playing,
    Paused,
    Stopped
}