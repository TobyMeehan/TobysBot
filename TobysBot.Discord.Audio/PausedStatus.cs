using System;

namespace TobysBot.Discord.Audio
{
    public class PausedStatus : ITrackStatus
    {
        public PausedStatus(IActiveTrack currentTrack)
        {
            CurrentTrack = currentTrack;
        }
        
        public virtual IActiveTrack CurrentTrack { get; }

        public override string ToString()
        {
            return "Paused";
        }
    }
}