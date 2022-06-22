using Discord;
using Discord.Commands;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using Fergun.Interactive.Selection;
using TobysBot.Commands.Response;
using TobysBot.Extensions;

namespace TobysBot.Commands.Modules;

public class HelpModule : CommandModuleBase
{
    private readonly EmbedService _embeds;
    private readonly ICommandService _commands;
    private readonly InteractiveService _interactions;
    
    private static IEmote OkEmote => new Emoji("👌");
    
    public HelpModule(EmbedService embeds, ICommandService commands, InteractiveService interactions)
    {
        _embeds = embeds;
        _commands = commands;
        _interactions = interactions;
    }

    private bool _responded = false;

    [Command("help", RunMode = RunMode.Async)]
    [Summary("Get information about the bot or a specific plugin.")]
    public async Task MainAsync(
        [Summary("Plugin.")] string? plugin = null)
    {
        await (plugin is null
            ? Home()
            : Plugin(plugin));
    }

    private async Task Home()
    {
        var page = new PageBuilder()
                .WithTitle("Toby's Bot Commands")
                .WithDescription(
                    "Toby's Bot is a modular, expandable and open source Discord bot. For more information see [bot.tobymeehan.com](https://bot.tobymeehan.com). View my source code on [Github](https://github.com/TobyMeehan/TobysBot)")
                ;
        
        foreach (var plugin in _commands.Plugins.Take(25))
        {
            page.AddField(field =>
            {
                field.Name = plugin.Name;
                field.Value = $"`/help {plugin.Id}`";
                field.IsInline = true;
            });
        }

        var selection = new SelectionBuilder<string>()
            .WithOptions(_commands.Plugins.Select(x => x.Name).OfType<string>().ToList())
            .WithInputType(InputType.SelectMenus)
            .WithSelectionPage(page)
            .WithActionOnTimeout(ActionOnStop.DisableInput)
            .WithActionOnSuccess(ActionOnStop.DisableInput)
            .Build();

        await Response.ReactAsync(OkEmote);
        _responded = true;
        
        var result = await _interactions.SendSelectionAsync(selection, Context.Channel, TimeSpan.FromMinutes(2));

        if (!result.IsSuccess)
        {
            return;
        }
        
        var selectedPlugin = _commands.Plugins.First(x => x.Name == result.Value);

        await Plugin(selectedPlugin.Id);
    }

    private async Task Plugin(string? plugin)
    {
        var pluginInfo = _commands.Plugins.FirstOrDefault(x => x.Id == plugin);

        if (pluginInfo is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription($"No results for `{plugin}` plugin.")
                .Build());

            return;
        }

        var usages = pluginInfo.Commands.SelectMany(x => x.Usages).ToList();

        var paginator = new LazyPaginatorBuilder()
            .WithPageFactory(GeneratePage)
            .WithMaxPageIndex(usages.Count / 25)
            .WithOptions(new Dictionary<IEmote, PaginatorAction>
            {
                { new Emoji("◀"), PaginatorAction.Backward },
                { new Emoji("▶"), PaginatorAction.Forward }
            })
            .WithCacheLoadedPages(false)
            .WithActionOnTimeout(ActionOnStop.DisableInput)
            .Build();

        if (!_responded)
        {
            await Response.ReactAsync(OkEmote);
        }

        await _interactions.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(2));

        PageBuilder GeneratePage(int index)
        {
            var page = new PageBuilder()
                .WithTitle($"{pluginInfo.Name} Plugin")
                .WithDescription(pluginInfo.Description!);
            
            foreach (var usage in usages.ForPage(index, 25))
            {
                page.AddField(field =>
                {
                    field.Name = $"/{usage.CommandName} {string.Join(", ", usage.Parameters.Select(x => $"[{x}]"))}";
                    field.Value = usage.Description;
                });
            }

            return page;
        }
    }
}