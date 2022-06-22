using Discord;

namespace TobysBot.Commands.Response;

public interface ISocketResponseService
{
    Task<ISocketResponse> ReplyAsync(string? text = null, bool isTTS = false, Visibility visibility = Visibility.Public, Embed? embed = null,
        AllowedMentions? allowedMentions = null, RequestOptions? options = null, MessageComponent? components = null,
        ISticker[]? stickers = null, Embed[]? embeds = null);

    Task<ISocketResponse> DeferAsync(bool ephemeral = false, RequestOptions? options = null);
    
    Task ReactAsync(IEmote emote, Visibility visibility = Visibility.Public, RequestOptions? options = null);
}