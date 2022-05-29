using Discord.Commands;

namespace TobysBot.Commands.Modules;

public class PongModule : CommandModuleBase
{
    [Command("ping")]
    [Summary("pong")]
    public async Task PingAsync()
    {
        await Response.ReplyAsync("pong");
    }

    [Command("say")]
    [Summary("Say something")]
    public async Task SayAsync(
        [Summary("Message to say.")]
        [Remainder] string message)
    {
        await Response.ReplyAsync(message);
    }
}