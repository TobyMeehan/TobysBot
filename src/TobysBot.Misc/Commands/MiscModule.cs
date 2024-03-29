﻿using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Commands.Response;

namespace TobysBot.Misc.Commands;

[Plugin("misc")]
public class MiscModule : CommandModuleBase
{
    private static IEmote OkEmote => new Emoji("👌");

    [Command("say")]
    [Summary("Says a message.")]
    public async Task SayAsync(
        [Summary("Message to say.")]
        [Remainder] string message)
    {
        await Response.ReactAsync(OkEmote);

        await Context.Channel.SendMessageAsync(message);
    }

    [Command("sayh")]
    [Summary("Says a message and hides your command.")]
    public async Task SayAndHideAsync(
        [Summary("Message to say.")] 
        [Remainder] string message)
    {
        await Response.ReactAsync(OkEmote, Visibility.Hidden);

        await Context.Channel.SendMessageAsync(message);

        if (Context.Message is not null)
        {
            await Context.Message.DeleteAsync();
        }
    }

    [Command("random")]
    [Alias("rng")]
    [Summary("Generates a random number.")]
    public async Task RandomAsync(
        [Summary("First value.")]
        long first = 100,
        [Summary("Second value.")]
        long second = 0)
    {
        if (first == second)
        {
            await Response.ReplyAsync($"What. I guess you are looking for {first}.");
            return;
        }

        (long max, long min) = (Math.Max(first, second), Math.Min(first, second));

        if (max is long.MaxValue)
        {
            max -= 1;
        }

        long result = Random.Shared.NextInt64(min, max + 1);

        await Response.ReplyAsync(result.ToString());
    }
}