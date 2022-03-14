using System;

namespace TobysBot.Discord.Audio
{
    public class PlayingStatus : ITrackStatus
    {
        public PlayingStatus(IActiveTrack currentTrack)
        {
            CurrentTrack = currentTrack;
        }
        
        public virtual IActiveTrack CurrentTrack { get; }

        public override string ToString()
        {
            return "Playing";
        }
    }
}