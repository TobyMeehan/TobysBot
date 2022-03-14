using System;

namespace TobysBot.Discord.Audio
{
    public interface ITrackStatus : IPlayerStatus
    {
        IActiveTrack CurrentTrack { get; }
    }
}