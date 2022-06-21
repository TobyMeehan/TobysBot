using Discord;
using TobysBot.Commands;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;

namespace TobysBot.Voice.Commands;

public abstract class VoiceCommandModuleBase : CommandModuleBase
{
    private readonly IVoiceService _voiceService;

    public VoiceCommandModuleBase(IVoiceService voiceService)
    {
        _voiceService = voiceService;
    }

    protected IPlayerStatus Status => Context.Guild is not null ? _voiceService.Status(Context.Guild) : throw new NullReferenceException("Guild context is null.");

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
        if (Context.Guild is null)
        {
            throw new NullReferenceException("Guild context is null.");
        }
        
        await _voiceService.LeaveAsync(Context.Guild);
    }
}