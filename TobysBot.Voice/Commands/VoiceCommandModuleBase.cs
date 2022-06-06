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

    protected bool IsUserInVoiceChannel(bool sameChannel = false)
    {
        return IsUserInVoiceChannel(out _, sameChannel);
    }
    
    protected bool IsUserInVoiceChannel(out IVoiceState voiceState, bool sameChannel = false)
    {
        voiceState = Context.User as IVoiceState;

        if (voiceState?.VoiceChannel is null)
        {
            return false;
        }

        if (!sameChannel)
        {
            return true;
        }

        if (_voiceService.Status(Context.Guild) is not IConnectedStatus status)
        {
            return false;
        }

        return voiceState.VoiceChannel.Id == status.VoiceChannel.Id;
    }

    protected async Task<bool> EnsureUserInVoiceAsync(bool errorMessage = true, bool sameChannel = false, bool required = true)
    {
        if (IsUserInVoiceChannel(sameChannel: true))
        {
            return true;
        }
        
        // User is not in same channel
        
        if (!IsUserInVoiceChannel() && required)
        {
            if (errorMessage)
            {
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithJoinVoiceError()
                    .Build());
            }
            
            return false;
        }

        // User is in voice channel
        
        if (_voiceService.Status(Context.Guild) is IConnectedStatus && sameChannel)
        {
            if (errorMessage)
            {
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithJoinSameVoiceError()
                    .Build());
            }
            
            return false;
        }

        return true;
    }

    protected async Task<bool> JoinVoiceChannelAsync(bool errorMessage = true, bool moveChannel = true)
    {
        if (!await EnsureUserInVoiceAsync(errorMessage))
        {
            return false;
        }

        // user is in voice channel
        
        IsUserInVoiceChannel(out var voiceState);
        var status = _voiceService.Status(Context.Guild);

        if (status is IConnectedStatus && !moveChannel) // user and bot are in voice channels
        {
            return await EnsureUserInVoiceAsync(errorMessage, sameChannel: true); // make sure channels are the same
        }
        
        await _voiceService.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
        
        return true;

    }

    protected async Task LeaveVoiceChannelAsync()
    {
        await _voiceService.LeaveAsync(Context.Guild);
    }
}