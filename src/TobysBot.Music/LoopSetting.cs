namespace TobysBot.Music;

/// <summary>
/// Represents a loop mode.
/// </summary>
public interface ILoopSetting { }

/// <summary>
/// Looping is disabled.
/// </summary>
public class DisabledLoopSetting : ILoopSetting { }

/// <summary>
/// Looping is enabled.
/// </summary>
public class EnabledLoopSetting : ILoopSetting { }

/// <summary>
/// Looping is enabled for the current track.
/// </summary>
public class TrackLoopSetting : EnabledLoopSetting { }

/// <summary>
/// Looping is enabled for the queue.
/// </summary>
public class QueueLoopSetting : EnabledLoopSetting { }