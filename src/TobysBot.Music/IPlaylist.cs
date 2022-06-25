namespace TobysBot.Music;

/// <summary>
/// Represents a playlist of tracks.
/// </summary>
public interface IPlaylist
{
    /// <summary>
    /// Title of the playlist.
    /// </summary>
    string Title { get; }
    
    /// <summary>
    /// URL of the playlist.
    /// </summary>
    string Url { get; }
    
    /// <summary>
    /// The tracks in the playlist.
    /// </summary>
    IEnumerable<ITrack> Tracks { get; }
}