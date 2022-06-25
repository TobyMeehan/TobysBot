using Discord;

namespace TobysBot.Music;

/// <summary>
/// Represents a music track.
/// </summary>
public interface ITrack
{
    /// <summary>
    /// Title of the track.
    /// </summary>
    string Title { get; }
    
    /// <summary>
    /// Author of the track.
    /// </summary>
    string Author { get; }
    
    /// <summary>
    /// Url of the track.
    /// </summary>
    string Url { get; }
    
    /// <summary>
    /// Url of the track's audio.
    /// </summary>
    string AudioUrl { get; }
    
    /// <summary>
    /// Duration of the track.
    /// </summary>
    TimeSpan Duration { get; }
    
    /// <summary>
    /// User that queued the track.
    /// </summary>
    IUser RequestedBy { get; }
}