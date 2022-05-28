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
        if (ephemeral)
        {
            await _message.Author.SendMessageAsync(text, isTTS, embed, options, allowedMentions, components, embeds);
        }
        else
        {
            await _message.ReplyAsync(text, isTTS, embed, allowedMentions, options, components, stickers, embeds);
        }

        return new SocketTextCommandResponse(_message);
    }

    public async Task<ISocketResponse> DeferAsync(bool ephemeral = false, RequestOptions options = null)
    {
        if (ephemeral)
        {
            return new SocketTextCommandResponse(_message);
        }
        
        return new SocketDeferredTextCommandResponse(_message, _message.Channel.EnterTypingState(options));
    }

    public async Task ReactAsync(IEmote emote, string text = null, bool isTTS = false, bool ephemeral = false, Embed embed = null,
        AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null,
        ISticker[] stickers = null, Embed[] embeds = null)
    {
        await _message.AddReactionAsync(emote, options);
    }
}