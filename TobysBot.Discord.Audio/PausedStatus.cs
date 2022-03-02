using System;

namespace TobysBot.Discord.Audio
{
    public class PausedStatus : PlayingStatus
    {
        public PausedStatus(ITrack track, TimeSpan position, TimeSpan duration) : base(track, position, duration)
        {
        }
    }
}