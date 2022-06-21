using Discord;
using Discord.WebSocket;

namespace TobysBot.Commands.Response;

public class SocketDeferredTextCommandResponse : SocketTextCommandResponse
{
    private readonly SocketUserMessage _command;
    private readonly IDisposable _typing;

    public SocketDeferredTextCommandResponse(SocketUserMessage command, bool ephemeral, IDisposable typing) : base(command, null)
    {
        _command = command;
        _typing = typing;
    }

    public override async Task ModifyResponseAsync(Action<MessageProperties> func, RequestOptions options = null)
    {
        var message = new MessageProperties();
        func(message);
        
        await _command.ReplyAsync(
            text: message.Content.IsSpecified ? message.Content.Value : null,
            embed: message.Embed.IsSpecified ? message.Embed.Value : null,
            allowedMentions: message.AllowedMentions.IsSpecified ? message.AllowedMentions.Value : null,
            components: message.Components.IsSpecified ? message.Components.Value : null,
            embeds: message.Embeds.IsSpecified ? message.Embeds.Value : null,
            options: options);

        _typing.Dispose();
    }

    public override Task ReactAsync(IEmote emote, RequestOptions options = null)
    {
        _typing.Dispose();

        return base.ReactAsync(emote, options);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _typing.Dispose();
        }
        
        base.Dispose(disposing);
    }
}