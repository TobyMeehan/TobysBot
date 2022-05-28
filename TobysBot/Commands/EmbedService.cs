using Discord;
using Microsoft.Extensions.Options;
using TobysBot.Configuration;

namespace TobysBot.Commands;

public class EmbedService
{
    private readonly TobysBotOptions _options;

    public EmbedService(IOptions<TobysBotOptions> options)
    {
        _options = options.Value;
    }

    public EmbedBuilder Action()
    {
        return new EmbedBuilder()
            .WithColor(new Color(_options.Embeds.Colors.Action));
    }

    public EmbedBuilder Information()
    {
        return new EmbedBuilder()
            .WithColor(new Color(_options.Embeds.Colors.Information));
    }

    public EmbedBuilder Error()
    {
        return new EmbedBuilder()
            .WithColor(new Color(_options.Embeds.Colors.Information));
    }
}