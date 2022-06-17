using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Commands;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;

namespace TobysBot.Voice.Commands;

public class CheckVoiceAttribute : CommandPreconditionAttribute
{
    public CheckVoiceAttribute()
    {
        
    }

    public CheckVoiceAttribute(bool showError = true, SameChannel sameChannel = SameChannel.NotRequired)
    {
        ShowError = showError;
        SameChannel = sameChannel;
    }
    public bool ShowError { get; set; } = true;
    public SameChannel SameChannel { get; set; }

    public override Task<PreconditionResult> CheckPermissionsAsync(SocketGenericCommandContext context, CommandInfo command, IServiceProvider services)
    {
        var voice = services.GetRequiredService<IVoiceService>();

        if (!context.User.IsInVoiceChannel(out var voiceState))
        {
            return Task.FromResult(PreconditionResult.FromError("Please join a voice channel."));
        }
        
        var currentVoiceChannel =
            voice.Status(context.Guild) is IConnectedStatus connected ? connected.VoiceChannel : null;

        var sameChannelCriteria = SameChannel switch
        {
            SameChannel.IfBotConnected when currentVoiceChannel is null => true,
            SameChannel.Required or SameChannel.IfBotConnected when voiceState.VoiceChannel?.Id !=
                                                                    currentVoiceChannel?.Id => false,
            _ => true
        };

        return Task.FromResult(!sameChannelCriteria 
            ? PreconditionResult.FromError("Please join the same voice channel.") 
            : PreconditionResult.FromSuccess());
    }
}