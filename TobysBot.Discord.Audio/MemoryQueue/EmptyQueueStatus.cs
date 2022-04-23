using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TobysBot.Discord.Audio.MemoryQueue;

public class EmptyQueueStatus : IQueueStatus
{
    public IEnumerable<ITrack> Previous => null;
    public IEnumerable<ITrack> Next => null;
    public IActiveTrack CurrentTrack => null;
    public LoopSetting LoopEnabled => new DisabledLoopSetting();
    public ShuffleSetting ShuffleEnabled => new DisabledShuffleSetting();
    public int Count => 0;

    public IEnumerator<ITrack> GetEnumerator() => Enumerable.Empty<ITrack>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    
}