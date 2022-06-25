namespace TobysBot.Music;

/// <summary>
/// Represents a queue of tracks.
/// </summary>
public interface IQueue : IEnumerable<ITrack>
{
    /// <summary>
    /// All the tracks in the queue.
    /// </summary>
    IEnumerable<ITrack> Tracks { get; }
    
    /// <summary>
    /// The tracks before the current track.
    /// </summary>
    IEnumerable<ITrack> Previous { get; }
    
    /// <summary>
    /// The tracks after the current track.
    /// </summary>
    IEnumerable<ITrack> Next { get; }
    
    /// <summary>
    /// The current track.
    /// </summary>
    IActiveTrack? CurrentTrack { get; }
    
    /// <summary>
    /// The current loop mode.
    /// </summary>
    ILoopSetting Loop { get; }
    
    /// <summary>
    /// Whether shuffle is enabled.
    /// </summary>
    bool Shuffle { get; }
    
    /// <summary>
    /// The length of the queue.
    /// </summary>
    int Length { get; }
    
    /// <summary>
    /// Whether the queue is empty.
    /// </summary>
    bool Empty { get; }
}