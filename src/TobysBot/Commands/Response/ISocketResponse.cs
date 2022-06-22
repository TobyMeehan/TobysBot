using Discord;

namespace TobysBot.Commands.Response;

public interface ISocketResponse : IDisposable
{
    Task ModifyResponseAsync(Action<MessageProperties> func, RequestOptions? options = null);

    Task FollowupResponseAsync(string? text = null, bool isTTS = false, Visibility visibility = Visibility.Public, Embed? embed = null,
        AllowedMentions? allowedMentions = null, RequestOptions? options = null, MessageComponent? components = null,
        ISticker[]? stickers = null, Embed[]? embeds = null);

    Task ReactAsync(IEmote emote, RequestOptions? options = null);
}