using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TobysBot.Discord.Client.TextCommands.Modules;

[Name("Random")]
[HelpCategory("random")]
public class RandomModule : ModuleBase<SocketCommandContext>
{
    [Command("random")]
    [Alias("rng")]
    [Summary("Generates a random number between min and max.")]
    public async Task RandomAsync(int min, int max)
    {
        if (min == max)
        {
            await Context.Message.ReplyAsync($"What. I guess you are looking for {min}");
        }
        
        if (max < min)
        {
            (max, min) = (min, max);
        }
        
        var rng = new Random();
        var result = rng.Next(min, max + 1);

        await Context.Message.ReplyAsync(result.ToString());
    }
}