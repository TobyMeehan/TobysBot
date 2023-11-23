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
public class TrackLoopSetting : EnabledLoopSetting
{ 
    public TrackLoopSetting()
    {
        Start = null;
        End = null;
    }
    
    public TrackLoopSetting(TimeSpan? start = null, TimeSpan? end = null)
    {
        Start = start;
        End = end;
    }
    
    public TimeSpan? Start { get; }
    public TimeSpan? End { get; }
}

/// <summary>
/// Looping is enabled for the queue.
/// </summary>
public class QueueLoopSetting : EnabledLoopSetting { }