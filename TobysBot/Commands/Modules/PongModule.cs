using Discord.Commands;

namespace TobysBot.Commands.Modules;

public class PongModule : DefaultModuleBase
{
    [Command("ping")]
    public async Task PingAsync()
    {
        await Response.ReplyAsync("pong");
    }
}