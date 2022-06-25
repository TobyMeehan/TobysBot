using Discord;

namespace TobysBot.Commands.Response;

/// <summary>
/// Represents a response which has been sent.
/// </summary>
public interface ISocketResponse : IDisposable
{
    /// <summary>
    /// Modifies the original response with the specified <see cref="MessageProperties"/>.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task ModifyResponseAsync(Action<MessageProperties> func, RequestOptions? options = null);

    /// <summary>
    /// Follows up the original response with another message.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="isTTS"></param>
    /// <param name="visibility"></param>
    /// <param name="embed"></param>
    /// <param name="allowedMentions"></param>
    /// <param name="options"></param>
    /// <param name="components"></param>
    /// <param name="stickers"></param>
    /// <param name="embeds"></param>
    /// <returns></returns>
    Task FollowupResponseAsync(string? text = null, bool isTTS = false, Visibility visibility = Visibility.Public, Embed? embed = null,
        AllowedMentions? allowedMentions = null, RequestOptions? options = null, MessageComponent? components = null,
        ISticker[]? stickers = null, Embed[]? embeds = null);

    /// <summary>
    /// Reacts to the original command.
    /// </summary>
    /// <param name="emote"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task ReactAsync(IEmote emote, RequestOptions? options = null);
}