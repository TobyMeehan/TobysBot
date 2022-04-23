using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using TobysBot.Discord.Audio;
using TobysBot.Discord.Audio.Status;
using TobysBot.Discord.Client.Configuration;
using TobysBot.Discord.Client.TextCommands.Extensions;

namespace TobysBot.Discord.Client.TextCommands.Modules;

[HelpCategory("misc")]
[Name("Miscellaneous")]
public class MiscModule : VoiceModuleBase
{
    private readonly StarOptions _starOptions;
    private readonly DiscordClientOptions _options;

    public MiscModule(IOptions<DiscordClientOptions> options, IOptions<StarOptions> starOptions, IAudioNode node) : base(node)
    {
        _starOptions = starOptions.Value;
        _options = options.Value;
    }

    private string Star(string star) => $"{star} is a star. S T A R  S T A R";

    [Command("pop")]
    [Summary("Calls the user a pop pop head.")]
    public async Task PopAsync(IUser user = null)
    {
        if (user is null || user.Id == Context.User.Id)
        {
            await Context.Message.ReplyAsync("You are a pop pop head.");
            return;
        }

        if (user.Id == Context.Client.CurrentUser.Id)
        {
            await Context.Message.ReplyAsync("I am not a pop pop head, how dare you!");
            return;
        }

        if (user.Id == _options.TobyId)
        {
            await ReplyAsync(Star(user.Mention));
        }
        else
        {
            await ReplyAsync($"{user.Mention} is a pop pop head.");
        }

        await Context.Message.DeleteAsync();
    }


    [Command("star")]
    [Summary("Declares the user an S T A R")]
    public async Task StarAsync(IUser user = null)
    {
        if (user is null)
        {
            var star = _starOptions.Stars.SelectRandom();

            await Context.Channel.SendMessageAsync(Star(star));

            return;
        }

        if (user.Id == Context.Client.CurrentUser.Id)
        {
            await Context.Channel.SendMessageAsync("I am a star. S T A R  S T A R");
            return;
        }
        if (user.Id == Context.User.Id)
        {
            await Context.Channel.SendMessageAsync("You are a star. S T A R  S T A R");
            return;
        }

        await Context.Channel.SendMessageAsync(Star(user.Mention));

        await Context.Message.DeleteAsync();
    }

    [Command("summon")]
    [Summary("Summons the user to your activity.")]
    public async Task SummonAsync(IUser user = null, IRole role = null)
    {
        IMentionable mention = role;
        user ??= Context.Client.CurrentUser;
        mention ??= user;

        if (mention is IUser)
        {
            if (user.Id == Context.Client.CurrentUser.Id)
            {
                if (IsUserInVoiceChannel(out var voice) && (PlayerStatus as IConnectedStatus)?.Channel.Id != voice.VoiceChannel.Id)
                {
                    await EnsureUserInVoiceAsync(false, true);
                    return;
                }
            
                await Context.Message.ReplyAsync("Do not worry, I am here.");
                return;
            }
            
            if (user.Id == Context.User.Id)
            {
                await Context.Message.ReplyAsync("Consider yourself summoned.");
                return;
            }
        }

        if (Context.User.Activities.Any())
        {
            var activity = Context.User.Activities.First();

            await ReplyAsync($"{mention.Mention}, {Context.User.Mention} wants you to join them in {activity.Name}");
        }
        else if (Context.User is IVoiceState voiceState && voiceState.VoiceChannel?.GuildId == Context.Guild.Id)
        {
            if (voiceState.IsStreaming)
            {
                await ReplyAsync(
                    $"{user.Mention}, {Context.User.Mention} wants you to watch their stream in {voiceState.VoiceChannel.Mention}.");
            }
            else
            {
                await ReplyAsync(
                    $"{user.Mention}, {Context.User.Mention} wants you to join them in {voiceState.VoiceChannel.Mention}");
            }

            if (voiceState.IsSelfDeafened || voiceState.IsSelfMuted)
            {
                await ReplyAsync($"{Context.User.Mention} you should probably unmute though...");
            }
        }
        else
        {
            await ReplyAsync(
                $"{user.Mention}, {Context.User.Mention} wants you to join them in whatever they are doing.");
        }

        await Context.Message.DeleteAsync();
    }

    [Command("say")]
    [Summary("Says a message.")]
    public async Task SayAsync([Remainder] string message)
    {
        await Context.Channel.SendMessageAsync(message);
    }

    [Command("sayh")]
    [Summary("Says a message and hides your command.")]
    public async Task SayAndHideAsync([Remainder] string message)
    {
        await SayAsync(message);

        await Context.Message.DeleteAsync();
    }
}