using Discord;

namespace TobysBot.Discord.Audio.Status;

public interface IConnectedStatus : IPlayerStatus
{
    IVoiceChannel Channel { get; }
}