using Discord;
using TobysBot.Commands;
using TobysBot.Commands.Modules;
using TobysBot.Extensions;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;

namespace TobysBot.Voice.Commands;

public abstract class VoiceCommandModuleBase : CommandModuleBase
{
    private readonly IVoiceService _voiceService;
    private readonly EmbedService _embeds;

    public VoiceCommandModuleBase(IVoiceService voiceService, EmbedService embeds)
    {
        _voiceService = voiceService;
        _embeds = embeds;
    }

    protected IPlayerStatus Status => _voiceService.Status(Context.Guild);

    protected async Task JoinVoiceChannelAsync()
    {
        if (!Context.User.IsInVoiceChannel(out var voiceState))
        {
            return;
        }

        await _voiceService.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
    }

    protected async Task LeaveVoiceChannelAsync()
    {
        await _voiceService.LeaveAsync(Context.Guild);
    }
}