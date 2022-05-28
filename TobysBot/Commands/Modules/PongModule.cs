using Discord.Commands;

namespace TobysBot.Commands.Modules;

public class PongModule : DefaultModuleBase
{
    [Command("ping")]
    [Summary("pong")]
    public async Task PingAsync()
    {
        await Response.ReplyAsync("pong");
    }
}