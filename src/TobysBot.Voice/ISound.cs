namespace TobysBot.Voice;

/// <summary>
/// Represents a sound which can be played.
/// </summary>
public interface ISound
{
    /// <summary>
    /// Name of the sound.
    /// </summary>
    string Title { get; }
    
    /// <summary>
    /// Url of the sound.
    /// </summary>
    string Url { get; }
    
    /// <summary>
    /// Duration of the sound.
    /// </summary>
    TimeSpan Duration { get; }
}