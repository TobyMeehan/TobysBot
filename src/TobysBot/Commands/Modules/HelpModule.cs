using Discord;
using Discord.Commands;
using TobysBot.Extensions;

namespace TobysBot.Commands.Modules;

public class HelpModule : CommandModuleBase
{
    private readonly EmbedService _embeds;
    private readonly CommandService _commands;

    public HelpModule(EmbedService embeds, CommandService commands)
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

        foreach (var module in _commands.Plugins().Take(25))
        {
            embed.AddField(field =>
            {
                field.Name = module.Name;
                field.Value = $"`/help {module.Id()}`";
                field.IsInline = true;
            });
        }

        return embed;
    }

    private EmbedBuilder Plugin(string plugin)
    {
        var pluginInfo = _commands.Plugins().FirstOrDefault(x => x.Id() == plugin);

        if (pluginInfo is null)
        {
            return _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription($"No results for `{plugin}` plugin.");
        }

        var embed = _embeds.Builder()
            .WithContext(EmbedContext.Information)
            .WithAuthor($"{pluginInfo.Name} Plugin")
            .WithDescription(pluginInfo.Summary);
        
        foreach (var command in pluginInfo.GetAllCommands().Take(25))
        {
            embed.AddField(field =>
            {
                field.Name = $"/{command.Aliases[0]}";
                field.Value = command.Summary;
            });
        }

        return embed;
    }
}