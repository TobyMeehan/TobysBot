using Discord;
using TobysBot.Commands;
using TobysBot.Commands.Modules;
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

    protected bool IsUserInVoiceChannel(out IVoiceState voiceState)
    {
        voiceState = Context.User as IVoiceState;
        return voiceState?.VoiceChannel is not null;
    }

    protected bool IsUserInSameVoiceChannel(out IVoiceState voiceState)
    {
        if (!IsUserInVoiceChannel(out voiceState))
        {
            return false;
        }

        if (_voiceService.Status(Context.Guild) is not IConnectedStatus status)
        {
            return false;
        }

        return voiceState.VoiceChannel.Id == status.VoiceChannel.Id;
    }

    protected async Task<bool> EnsureUserInSameVoiceAsync()
    {
        if (!IsUserInSameVoiceChannel(out _))
        {
            await Response.ReplyAsync(embed: _embeds.Error()
                .WithDescription("We need to be in the same voice channel to do that.")
                .Build());

            return false;
        }

        return true;
    }
    
    protected async Task JoinVoiceChannelAsync(bool required = true, bool moveChannel = true)
    {
        if (!IsUserInVoiceChannel(out var voiceState))
        {
            if (required)
            {
                await Response.ReplyAsync(embed: _embeds.Error()
                    .WithDescription("Join the voice channel you square.")
                    .Build());
            }

            return;
        }

        if (_voiceService.Status(Context.Guild) is IConnectedStatus status &&
            status.VoiceChannel.Id != voiceState.VoiceChannel.Id && !moveChannel)
        {
            return;
        }

        await _voiceService.JoinAsync(voiceState.VoiceChannel);
    }

    protected async Task LeaveVoiceChannelAsync()
    {
        await _voiceService.LeaveAsync(Context.Guild);
    }
}