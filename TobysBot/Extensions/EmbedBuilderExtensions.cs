using Discord;
using TobysBot.Commands;

namespace TobysBot.Extensions;

public static class EmbedBuilderExtensions
{
    public static EmbedBuilder WithContext(this EmbedBuilder builder, EmbedContext context)
    {
        if (builder is not EmbedServiceBuilder embed)
        {
            throw new Exception("Invalid builder type.");
        }

        var color = new Color(context switch
        {
            EmbedContext.Action => embed.Service.EmbedOptions.Colors.Action,
            EmbedContext.Information => embed.Service.EmbedOptions.Colors.Information,
            EmbedContext.Error => embed.Service.EmbedOptions.Colors.Error
        });

        return embed.WithColor(color);
    }
}