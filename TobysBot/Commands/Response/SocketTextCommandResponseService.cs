using Discord;
using Discord.WebSocket;

namespace TobysBot.Commands.Response;

public class SocketTextCommandResponseService : ISocketResponseService
{
    private readonly SocketUserMessage _message;

    public SocketTextCommandResponseService(SocketUserMessage message)
    {
        _message = message;
    }
    
    public async Task<ISocketResponse> ReplyAsync(string text = null, bool isTTS = false, bool ephemeral = false, Embed embed = null,
        AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null,
        ISticker[] stickers = null, Embed[] embeds = null)
    {
        var response = ephemeral
            ? await _message.Author.SendMessageAsync(text, isTTS, embed, options, allowedMentions, components, embeds)
            : await _message.ReplyAsync(text, isTTS, embed, allowedMentions, options, components, stickers, embeds);

        return new SocketTextCommandResponse(_message, response);
    }

    public async Task<ISocketResponse> DeferAsync(bool ephemeral = false, RequestOptions options = null)
    {
        return new SocketDeferredTextCommandResponse(_message, ephemeral, _message.Channel.EnterTypingState(options));
    }

    public async Task ReactAsync(IEmote emote, RequestOptions options = null)
    {
        await _message.AddReactionAsync(emote, options);
    }
}