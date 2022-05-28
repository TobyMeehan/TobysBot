using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace TobysBot.Commands.Response;

public class SocketTextCommandResponse : ISocketResponse
{
    private readonly SocketUserMessage _message;

    public SocketTextCommandResponse(SocketUserMessage message)
    {
        _message = message;
    }
    
    public virtual async Task ModifyResponseAsync(Action<MessageProperties> func, RequestOptions options = null)
    {
        await _message.ModifyAsync(func, options);
    }

    public async Task FollowupResponseAsync(string text = null, bool isTTS = false, bool ephemeral = false, Embed embed = null,
        AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null,
        ISticker[] stickers = null, Embed[] embeds = null)
    {
        await _message.ReplyAsync(text, isTTS, embed, allowedMentions, options, components, stickers, embeds);
    }
}