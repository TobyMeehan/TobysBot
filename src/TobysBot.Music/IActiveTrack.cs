namespace TobysBot.Music;

/// <summary>
/// Represents a track currently playing.
/// </summary>
public interface IActiveTrack : ITrack
{
    /// <summary>
    /// Position of the track.
    /// </summary>
    TimeSpan Position { get; }
    
    /// <summary>
    /// Status of the track.
    /// </summary>
    ActiveTrackStatus Status { get; }
    
    /// <summary>
    /// The track currently playing.
    /// </summary>
    ITrack InnerTrack { get; }
}

/// <summary>
/// Represents the status of an active track.
/// </summary>
public enum ActiveTrackStatus
{
    /// <summary>
    /// The track is playing.
    /// </summary>
    Playing,
    
    /// <summary>
    /// The track is paused.
    /// </summary>
    Paused,
    
    /// <summary>
    /// The player is stopped.
    /// </summary>
    Stopped
}