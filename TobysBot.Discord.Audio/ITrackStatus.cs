using System;

namespace TobysBot.Discord.Audio
{
    public interface ITrackStatus : IPlayerStatus
    {
        ITrack CurrentTrack { get; }
        
        TimeSpan Position { get; }
        
        TimeSpan Duration { get; }
    }
}