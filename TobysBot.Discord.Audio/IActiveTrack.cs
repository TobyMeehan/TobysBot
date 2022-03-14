using System;

namespace TobysBot.Discord.Audio;

public interface IActiveTrack : ITrack
{
    TimeSpan Position { get; }
}