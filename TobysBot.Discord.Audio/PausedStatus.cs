using System;

namespace TobysBot.Discord.Audio
{
    public class PausedStatus : ITrackStatus
    {
        public PausedStatus(ITrack track, TimeSpan position, TimeSpan duration)
        {
            CurrentTrack = track;
            Position = position;
            Duration = duration;
        }
        
        public ITrack CurrentTrack { get; }
        
        public TimeSpan Position { get; }
        
        public TimeSpan Duration { get; }
    }
}