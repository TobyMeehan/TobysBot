using TobysBot.Data;

namespace TobysBot.Music;

/// <summary>
/// Represents a saved queue.
/// </summary>
public interface ISavedQueue : IEntity
{
    /// <summary>
    /// Name of the saved queue.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// User the saved queue belongs to.
    /// </summary>
    ulong UserId { get; }
    
    /// <summary>
    /// The tracks in the saved queue.
    /// </summary>
    IEnumerable<ITrack> Tracks { get; }
}