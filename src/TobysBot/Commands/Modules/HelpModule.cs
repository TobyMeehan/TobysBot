using Discord;
using Discord.Commands;
using TobysBot.Extensions;

namespace TobysBot.Commands.Modules;

public class HelpModule : CommandModuleBase
{
    private readonly EmbedService _embeds;
    private readonly ICommandService _commands;

    public HelpModule(EmbedService embeds, ICommandService commands)
    {
        _embeds = embeds;
        _commands = commands;
    }
    
    [Command("help")]
    [Summary("Get information about the bot or a specific plugin.")]
    public async Task MainAsync(
        [Summary("Plugin.")]
        string? plugin = null)
    {
        var builder = plugin is null
            ? Home()
            : Plugin(plugin);

        var embed = builder.Build();

        await Response.ReplyAsync(embed: embed);
    }

    private EmbedBuilder Home()
    {
        var embed = _embeds.Builder()
            .WithAuthor("Toby's Bot Commands")
            .WithContext(EmbedContext.Information)
            .WithDescription("Toby's Bot is a modular, expandable and open source Discord bot. For more information see [bot.tobymeehan.com](https://bot.tobymeehan.com). View my source code on [Github](https://github.com/TobyMeehan/TobysBot)");

        foreach (var plugin in _commands.Plugins.Take(25))
        {
            embed.AddField(field =>
            {
                field.Name = plugin.Name;
                field.Value = $"`/help {plugin.Id}`";
                field.IsInline = true;
            });
        }

        return embed;
    }

    private EmbedBuilder Plugin(string plugin)
    {
        var pluginInfo = _commands.Plugins.FirstOrDefault(x => x.Id == plugin);

        if (pluginInfo is null)
        {
            return _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription($"No results for `{plugin}` plugin.");
        }

        var embed = _embeds.Builder()
            .WithContext(EmbedContext.Information)
            .WithAuthor($"{pluginInfo.Name} Plugin")
            .WithDescription(pluginInfo.Description);
        
        foreach (var usage in pluginInfo.Commands.SelectMany(x => x.Usages).Take(25))
        {
            embed.AddField(field =>
            {
                field.Name = $"/{usage.CommandName} {string.Join(' ', usage.Parameters.Select(x => $"[{x}]"))}";
                field.Value = usage.Description;
            });
        }

        return embed;
    }
}