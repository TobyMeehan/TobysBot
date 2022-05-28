using Discord;

namespace TobysBot.Voice.Status;

public interface IConnectedStatus : IPlayerStatus
{
    IVoiceChannel VoiceChannel { get; }
    ITextChannel TextChannel { get; }
}