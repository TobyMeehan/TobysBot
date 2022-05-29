using Discord;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Voice.Configuration;

namespace TobysBot.Voice.Extensions;

public static class EmbedBuilderExtensions
{
    private static EmbedServiceBuilder ThrowIfNoService(EmbedBuilder builder)
    {
        if (builder is not EmbedServiceBuilder service)
        {
            throw new Exception("Invalid builder type.");
        }

        return service;
    }

    public static EmbedBuilder WithJoinVoiceError(this EmbedBuilder builder)
    {
        var embed = ThrowIfNoService(builder);

        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(embed.Service.Options<VoiceEmbedOptions>().JoinVoiceErrorDescription);
    }

    public static EmbedBuilder WithJoinSameVoiceError(this EmbedBuilder builder)
    {
        var embed = ThrowIfNoService(builder);

        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(embed.Service.Options<VoiceEmbedOptions>().JoinSameVoiceErrorDescription);
    }
}