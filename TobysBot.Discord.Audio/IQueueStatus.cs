using System.Collections.Generic;

namespace TobysBot.Discord.Audio
{
    public interface IQueueStatus : IEnumerable<ITrack>
    {
        IEnumerable<ITrack> Previous { get; }

        IEnumerable<ITrack> Next { get; }

        IActiveTrack CurrentTrack { get; }
        
        LoopSetting LoopEnabled { get; }
        
        ShuffleSetting ShuffleEnabled { get; }
    }
}