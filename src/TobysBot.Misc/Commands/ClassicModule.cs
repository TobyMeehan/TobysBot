using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using TobysBot.Commands;
using TobysBot.Commands.Response;
using TobysBot.Misc.Configuration;
using TobysBot.Misc.Extensions;

namespace TobysBot.Misc.Commands;

public class ClassicModule : CommandModuleBase
{
    private readonly StarOptions _options;

    private static IEmote OkEmote => new Emoji("👌");
    
    public ClassicModule(IOptions<StarOptions> options)
    {
        _options = options.Value;
    }
    
    private static string Star(IMentionable mention) => Star($"{mention.Mention} is");
    private static string Star(string pronoun) => $"{pronoun.Trim()} a star. S T A R  S T A R";

    [Command("pop")]
    [Summary("Calls the user a pop pop head.")]
    public async Task PopAsync(
        [Summary("User to call a pop pop head.")]
        IUser? user = null)
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

        await Response.ReactAsync(OkEmote, Visibility.Ephemeral);
        
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
        IUser? user = null)
    {
        await Response.ReactAsync(OkEmote, Visibility.Ephemeral);
        
        if (user is null)
        {
            string star = _options.StarNames.SelectRandom();

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

    [Command("summon")]
    [Summary("Summons the user to your activity.")]
    public async Task SummonAsync(
        [Summary("User to summon.")]
        IUser? user = null, 
        [Summary("Role to summon.")]
        IRole? role = null)
    {
        IMentionable? mention = role;
        user ??= Context.Client.CurrentUser;
        mention ??= user;

        if (mention is IUser)
        {
            if (user.Id == Context.Client.CurrentUser.Id)
            {
                await Response.ReplyAsync("Do not worry, I am here.");
                return;
            }

            if (user.Id == Context.User.Id)
            {
                await Context.Message.ReplyAsync("Consider yourself summoned.");
                return;
            }
        }

        await Response.ReactAsync(OkEmote, Visibility.Ephemeral);
        
        if (Context.User.Activities.Any())
        {
            var activity = Context.User.Activities.First();

            await ReplyAsync($"{mention.Mention}, {Context.User.Mention} wants you to join the in {activity.Name}");
        }
        else if (Context.Guild is not null && Context.User is IVoiceState voiceState && voiceState.VoiceChannel?.GuildId == Context.Guild.Id)
        {
            if (voiceState.IsStreaming)
            {
                await ReplyAsync(
                    $"{mention.Mention}, {Context.User.Mention} wants you to watch their stream in {voiceState.VoiceChannel.Mention}");
            }
            else
            {
                await ReplyAsync(
                    $"{mention.Mention}, {Context.User.Mention} wants you to join them in {voiceState.VoiceChannel.Mention}");
            }

            if (voiceState.IsSelfDeafened || voiceState.IsSelfMuted)
            {
                await ReplyAsync($"{Context.User.Mention} you should probably unmute yourself though...");
            }
        }
        else
        {
            await ReplyAsync(
                $"{mention.Mention}, {Context.User.Mention} wants you to join them in whatever they are doing.");
        }

        if (Context.Message is not null)
        {
            await Context.Message.DeleteAsync();
        }
    }
}