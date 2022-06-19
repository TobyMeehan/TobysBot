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

    public IEnumerable<ITrack> Tracks
    {
        get
        {
            var result = new List<ITrack>();

            result.AddRange(Previous);

            if (CurrentTrack is not null)
            {
                result.Add(CurrentTrack);
            }
            
            result.AddRange(Next);

            return result;
        }
    }
    public IEnumerable<ITrack> Previous { get; } = Enumerable.Empty<ITrack>();
    public IEnumerable<ITrack> Next { get; } = Enumerable.Empty<ITrack>();
    public IActiveTrack CurrentTrack { get; }
    public ILoopSetting Loop { get; } = new DisabledLoopSetting();
    public bool Shuffle { get; }

    public int Length => Tracks.Count();
    public bool Empty => Length < 1;
    
    public IEnumerator<ITrack> GetEnumerator()
    {
        return Tracks.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}