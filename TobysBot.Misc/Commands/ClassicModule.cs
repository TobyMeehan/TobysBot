using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using TobysBot.Commands;
using TobysBot.Commands.Modules;
using TobysBot.Commands.Response;
using TobysBot.Extensions;
using TobysBot.Misc.Configuration;
using TobysBot.Misc.Extensions;

namespace TobysBot.Misc.Commands;

public class ClassicModule : CommandModuleBase
{
    private readonly EmbedService _embeds;
    private readonly StarOptions _options;

    public ClassicModule(EmbedService embeds, IOptions<StarOptions> options)
    {
        _embeds = embeds;
        _options = options.Value;
    }
    
    private string Star(IMentionable mention) => Star($"{mention.Mention} is");
    private string Star(string pronoun) => $"{pronoun.Trim()} a star. S T A R  S T A R";

    [Command("pop")]
    [Summary("Calls the user a pop pop head.")]
    public async Task PopAsync(
        [Summary("User to call a pop pop head.")]
        IUser user = null)
    {
        if (user is null || user.Id == Context.User.Id)
        {
            await Response.ReplyAsync("You are a pop pop head.");
            return;
        }

        if (user.Id == Context.Client.CurrentUser.Id)
        {
            await Response.ReplyAsync("I am not a pop pop head, how dare you!");
            return;
        }

        await Response.ReplyAsync(visibility: Visibility.Hidden, embed: _embeds.Builder()
            .WithContext(EmbedContext.Action)
            .WithDescription("Popping!")
            .Build());
        
        if (_options.StarUsers.Contains(user.Id))
        {
            await ReplyAsync(Star($"{user.Mention} is"));
        }
        else
        {
            await ReplyAsync($"{user.Mention} is a pop pop head.");
        }

        if (Context.Message is not null)
        {
            await Context.Message.DeleteAsync();
        }
    }

    [Command("star")]
    [Summary("Declares the user an S T A R")]
    public async Task StarAsync(
        [Summary("Star to declare.")] 
        IUser user = null)
    {
        await Response.ReplyAsync(visibility: Visibility.Hidden, embed: _embeds.Builder()
            .WithContext(EmbedContext.Action)
            .WithDescription("Starring!")
            .Build());
        
        if (user is null)
        {
            var star = _options.StarNames.SelectRandom();

            await Context.Channel.SendMessageAsync(Star($"{star} is"));
            
            return;
        }

        if (user.Id == Context.Client.CurrentUser.Id)
        {
            await Context.Channel.SendMessageAsync(Star("I am"));
            return;
        }

        if (user.Id == Context.User.Id)
        {
            await Context.Channel.SendMessageAsync(Star("You are"));
            return;
        }

        await Context.Channel.SendMessageAsync(Star(user));

        if (Context.Message is not null)
        {
            await Context.Message.DeleteAsync();
        }
    }
}