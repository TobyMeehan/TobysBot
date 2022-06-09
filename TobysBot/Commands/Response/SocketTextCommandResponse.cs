using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace TobysBot.Commands.Response;

public class SocketTextCommandResponse : ISocketResponse
{
    private readonly SocketUserMessage _command;
    private readonly IUserMessage _response;

    public SocketTextCommandResponse(SocketUserMessage command, IUserMessage response)
    {
        _command = command;
        _response = response;
    }
    
    public virtual async Task ModifyResponseAsync(Action<MessageProperties> func, RequestOptions options = null)
    {
        await _response.ModifyAsync(func, options);
    }

    public async Task FollowupResponseAsync(string text = null, bool isTTS = false, bool ephemeral = false, Embed embed = null,
        AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null,
        ISticker[] stickers = null, Embed[] embeds = null)
    {
        await _response.ReplyAsync(text, isTTS, embed, allowedMentions, options, components, stickers, embeds);
    }

    public virtual async Task ReactAsync(IEmote emote, RequestOptions options = null)
    {
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