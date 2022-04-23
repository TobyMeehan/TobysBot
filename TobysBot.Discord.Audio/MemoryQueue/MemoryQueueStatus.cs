using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TobysBot.Discord.Audio.MemoryQueue;

public class MemoryQueueStatus : IQueueStatus
{
    private readonly IReadOnlyList<ITrack> _tracks;

    public MemoryQueueStatus(MemoryTrackCollection tracks)
    {
        _tracks = tracks.ToList();

        Previous = tracks.Played;
        CurrentTrack = tracks.CurrentTrack;
        Next = tracks.Queue;

        LoopEnabled = tracks.LoopEnabled;
        ShuffleEnabled = tracks.ShuffleEnabled;
    }

    public IEnumerable<ITrack> Previous { get; }
    public IEnumerable<ITrack> Next { get; }
    public IActiveTrack CurrentTrack { get; }
    
    public LoopSetting LoopEnabled { get; }
    public ShuffleSetting ShuffleEnabled { get; }
    public int Count => _tracks.Count;

    public IEnumerator<ITrack> GetEnumerator()
    {
        return _tracks.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}