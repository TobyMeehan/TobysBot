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

    public CheckVoiceAttribute(bool required = true, bool showError = true, bool sameChannel = false)
    {
        Required = required;
        ShowError = showError;
        SameChannel = sameChannel;
    }

    public bool Required { get; set; } = true;
    public bool ShowError { get; set; } = true;
    public bool SameChannel { get; set; }

    public override Task<PreconditionResult> CheckPermissionsAsync(SocketGenericCommandContext context, CommandInfo command, IServiceProvider services)
    {
        var voice = services.GetRequiredService<IVoiceService>();

        if (!context.User.IsInVoiceChannel(out var voiceState))
        {
            return Task.FromResult(PreconditionResult.FromError("Please join a voice channel."));
        }
        
        var currentVoiceChannel =
            voice.Status(context.Guild) is IConnectedStatus connected ? connected.VoiceChannel : null;

        if (SameChannel && voiceState.VoiceChannel.Id != currentVoiceChannel?.Id)
        {
            return Task.FromResult(PreconditionResult.FromError("Please join the same voice channel."));
        }
        
        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}