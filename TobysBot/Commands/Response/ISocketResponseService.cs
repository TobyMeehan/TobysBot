using Discord;

namespace TobysBot.Commands.Response;

public interface ISocketResponseService
{
    Task<ISocketResponse> ReplyAsync(string text = null, bool isTTS = false, bool ephemeral = false, Embed embed = null,
        AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null,
        ISticker[] stickers = null, Embed[] embeds = null);

    Task<ISocketResponse> DeferAsync(bool ephemeral = false, RequestOptions options = null);
    
    Task ReactAsync(IEmote emote, string text = null, bool isTTS = false, bool ephemeral = false, Embed embed = null,
        AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null,
        ISticker[] stickers = null, Embed[] embeds = null);
}