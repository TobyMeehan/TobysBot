namespace TobysBot.Music;

public interface IQueue
{
    IEnumerable<ITrack> Previous { get; }
    IEnumerable<ITrack> Next { get; }
    IActiveTrack CurrentTrack { get; }
    
    ILoopSetting Loop { get; }
    bool Shuffle { get; }
}