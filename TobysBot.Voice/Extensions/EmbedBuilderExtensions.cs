using Discord;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Voice.Configuration;

namespace TobysBot.Voice.Extensions;

public static class EmbedBuilderExtensions
{
    public static EmbedBuilder WithJoinVoiceAction(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Action)
            .WithDescription(x => x.Service.Options<VoiceOptions>().Embeds.JoinVoiceAction);
    }

    public static EmbedBuilder WithLeaveVoiceAction(this EmbedBuilder builder)
    {
        return builder
            .WithContext(EmbedContext.Action)
            .WithDescription(x => x.Service.Options<VoiceOptions>().Embeds.LeaveVoiceAction);
    }
    
    public static EmbedBuilder WithJoinVoiceError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<VoiceOptions>().Embeds.JoinVoiceErrorDescription);
    }

    public static EmbedBuilder WithJoinSameVoiceError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<VoiceOptions>().Embeds.JoinSameVoiceErrorDescription);
    }

    public static EmbedBuilder WithAlreadyBoundError(this EmbedBuilder embed, ITextChannel channel)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription($"Already bound to {channel.Mention}.");
    }

    public static EmbedBuilder WithRebindAction(this EmbedBuilder embed, ITextChannel channel)
    {
        return embed
            .WithContext(EmbedContext.Action)
            .WithDescription($"Player messages now bound to {channel.Mention}");
    }
}