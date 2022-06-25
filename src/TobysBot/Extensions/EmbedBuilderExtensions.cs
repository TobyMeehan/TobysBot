using Discord;
using TobysBot.Commands;

namespace TobysBot.Extensions;

public static class EmbedBuilderExtensions
{
    public static EmbedServiceBuilder GetWithService(this EmbedBuilder builder)
    {
        if (builder is not EmbedServiceBuilder service)
        {
            throw new Exception("Invalid builder type.");
        }

        return service;
    }
    
    public static EmbedBuilder WithContext(this EmbedBuilder builder, EmbedContext context)
    {
        if (builder is not EmbedServiceBuilder embed)
        {
            throw new Exception("Invalid builder type.");
        }

        var defaultColor = Color.LightGrey;
        
        var color = new Color(context switch
        {
            EmbedContext.Action => embed.Service.EmbedOptions?.Colors?.Action ?? defaultColor,
            EmbedContext.Information => embed.Service.EmbedOptions?.Colors?.Information ?? defaultColor,
            EmbedContext.Error => embed.Service.EmbedOptions?.Colors?.Error ?? defaultColor,
            _ => defaultColor
        });

        return embed.WithColor(color);
    }

    public static EmbedBuilder WithDescription(this EmbedBuilder embed, Func<EmbedServiceBuilder, string> description)
    {
        return embed
            .WithDescription(description(embed.GetWithService()));
    }
}