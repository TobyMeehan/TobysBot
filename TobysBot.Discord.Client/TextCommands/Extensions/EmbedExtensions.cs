using Discord;

namespace TobysBot.Discord.Client.TextCommands.Extensions;

public static class EmbedExtensions
{
    public static EmbedBuilder WithColorRed(this EmbedBuilder embed)
    {
        return embed.WithColor(new Color(5832704));
    }

    public static EmbedBuilder WithColorBlue(this EmbedBuilder embed)
    {
        return embed.WithColor(new Color(789332));
    }

    public static EmbedBuilder WithColorPurple(this EmbedBuilder embed)
    {
        return embed.WithColor(new Color(3278636));
    }

    public static EmbedBuilder WithContext(this EmbedBuilder embed, EmbedContext context)
    {
        return context switch
        {
            EmbedContext.Action => embed.WithColorRed(),
            EmbedContext.Error => embed.WithColorBlue(),
            EmbedContext.Information => embed.WithColorPurple(),
            _ => embed
        };
    }
}

public enum EmbedContext
{
    Action,
    Error,
    Information
}