using Discord.Commands;

namespace TobysBot.Commands.Modules;

public class PingModule : CommandModuleBase
{
    [Command("ping")]
    [Summary("pong")]
    public async Task PingAsync()
    {
        await Response.ReplyAsync("pong");
    }
}