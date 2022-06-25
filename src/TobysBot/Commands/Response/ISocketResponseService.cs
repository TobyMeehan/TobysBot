using Discord;

namespace TobysBot.Commands.Response;

/// <summary>
/// Service for responding to commands.
/// </summary>
public interface ISocketResponseService
{
    /// <summary>
    /// Replies to the command.
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
    Task<ISocketResponse> ReplyAsync(string? text = null, bool isTTS = false, Visibility visibility = Visibility.Public, Embed? embed = null,
        AllowedMentions? allowedMentions = null, RequestOptions? options = null, MessageComponent? components = null,
        ISticker[]? stickers = null, Embed[]? embeds = null);

    /// <summary>
    /// Defers the response, and responds with a loading message.
    /// </summary>
    /// <param name="ephemeral"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task<ISocketResponse> DeferAsync(bool ephemeral = false, RequestOptions? options = null);
    
    /// <summary>
    /// Reacts to the command.
    /// </summary>
    /// <param name="emote"></param>
    /// <param name="visibility"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task ReactAsync(IEmote emote, Visibility visibility = Visibility.Public, RequestOptions? options = null);
}