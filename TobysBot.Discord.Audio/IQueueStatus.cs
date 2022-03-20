using System.Collections.Generic;

namespace TobysBot.Discord.Audio
{
    public interface IQueueStatus : IEnumerable<ITrack>
    {
        IEnumerable<ITrack> Previous();

        IEnumerable<ITrack> Next();

        IActiveTrack CurrentTrack { get; }
        
        LoopSetting LoopEnabled { get; }
        
        ShuffleSetting ShuffleEnabled { get; }
    }
}