using Discord;

namespace TobysBot.Commands.Response;

public interface ISocketResponse
{
    Task ModifyResponseAsync(Action<MessageProperties> func, RequestOptions options = null);

    Task FollowupResponseAsync(string text = null, bool isTTS = false, bool ephemeral = false, Embed embed = null,
        AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null,
        ISticker[] stickers = null, Embed[] embeds = null);
}