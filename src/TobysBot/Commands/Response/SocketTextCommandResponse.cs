using Discord;
using Discord.WebSocket;

namespace TobysBot.Commands.Response;

public class SocketTextCommandResponse : ISocketResponse
{
    private readonly SocketUserMessage _command;
    private readonly IUserMessage? _response;

    public SocketTextCommandResponse(SocketUserMessage command, IUserMessage? response)
    {
        _command = command;
        _response = response;
    }
    
    public virtual async Task ModifyResponseAsync(Action<MessageProperties> func, RequestOptions? options = null)
    {
        if (_response is null)
        {
            return;
        }
        
        await _response.ModifyAsync(func, options);
    }

    public async Task FollowupResponseAsync(string? text = null, bool isTTS = false, Visibility visibility = Visibility.Public, Embed? embed = null,
        AllowedMentions? allowedMentions = null, RequestOptions? options = null, MessageComponent? components = null,
        ISticker[]? stickers = null, Embed[]? embeds = null)
    {
        switch (visibility)
        {
            case Visibility.Private:
                await _command.Author.SendMessageAsync(text, isTTS, embed, options, allowedMentions, components, embeds);
                break;
            case Visibility.Public or Visibility.Ephemeral:
                await _response.ReplyAsync(text, isTTS, embed, allowedMentions, options, components, stickers, embeds);
                break;
        }
    }

    public virtual async Task ReactAsync(IEmote emote, RequestOptions? options = null)
    {
        if (_response is null)
        {
            return;
        }
        
        await _response.AddReactionAsync(emote, options);
    }

    protected virtual void Dispose(bool disposing)
    {
        
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}