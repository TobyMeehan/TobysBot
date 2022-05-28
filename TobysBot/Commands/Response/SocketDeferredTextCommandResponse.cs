using Discord;
using Discord.WebSocket;

namespace TobysBot.Commands.Response;

public class SocketDeferredTextCommandResponse : SocketTextCommandResponse
{
    private readonly IDisposable _typing;

    public SocketDeferredTextCommandResponse(SocketUserMessage message, IDisposable typing) : base(message)
    {
        _typing = typing;
    }

    public override async Task ModifyResponseAsync(Action<MessageProperties> func, RequestOptions options = null)
    {
        await base.ModifyResponseAsync(func, options);

        _typing.Dispose();
    }
}