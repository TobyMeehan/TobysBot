using System.Collections;

namespace TobysBot.Music.MemoryQueue;

public class Queue : IQueue
{
    public Queue()
    {
        
    }
    
    public Queue(TrackCollection tracks)
    {
        Previous = tracks.Previous;
        Next = tracks.Next;

        CurrentTrack = tracks.CurrentTrack;

        Loop = tracks.LoopEnabled;
        Shuffle = tracks.Shuffle;
    }

    public IEnumerable<ITrack> Previous { get; } = Enumerable.Empty<ITrack>();
    public IEnumerable<ITrack> Next { get; } = Enumerable.Empty<ITrack>();
    public IActiveTrack CurrentTrack { get; }
    public ILoopSetting Loop { get; } = new DisabledLoopSetting();
    public bool Shuffle { get; }

    public int Length => Previous.Append(CurrentTrack).Concat(Next).Count();
}