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
    
    public async Task<ISocketResponse> ReplyAsync(string? text = null, bool isTTS = false, Visibility visibility = Visibility.Public, Embed? embed = null,
        AllowedMentions? allowedMentions = null, RequestOptions? options = null, MessageComponent? components = null,
        ISticker[]? stickers = null, Embed[]? embeds = null)
    {
        var response = visibility switch
        {
            Visibility.Private => await _message.Author.SendMessageAsync(text, isTTS, embed, options, allowedMentions,
                components, embeds),
            Visibility.Public or Visibility.Ephemeral => await _message.ReplyAsync(text, isTTS, embed, allowedMentions,
                options, components, stickers, embeds),
            _ => null
        };

        return new SocketTextCommandResponse(_message, response);
    }

    public Task<ISocketResponse> DeferAsync(bool ephemeral = false, RequestOptions? options = null)
    {
        return Task.FromResult<ISocketResponse>(new SocketDeferredTextCommandResponse(_message, ephemeral, _message.Channel.EnterTypingState(options)));
    }

    public async Task ReactAsync(IEmote emote, Visibility visibility = Visibility.Public, RequestOptions? options = null)
    {
        if (visibility is Visibility.Public or Visibility.Ephemeral)
        {
            await _message.AddReactionAsync(emote, options);
        }
    }
}