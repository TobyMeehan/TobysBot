using Discord;
using Discord.WebSocket;

namespace TobysBot.Commands.Response;

public class SocketSlashCommandResponse : ISocketResponse
{
    private readonly SocketSlashCommand _command;

    public SocketSlashCommandResponse(SocketSlashCommand command)
    {
        _command = command;
    }

    public async Task ModifyResponseAsync(Action<MessageProperties> func, RequestOptions options = null)
    {
        await _command.ModifyOriginalResponseAsync(func, options);
    }

    public async Task FollowupResponseAsync(string text = null, bool isTTS = false, bool ephemeral = false, Embed embed = null,
        AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null,
        ISticker[] stickers = null, Embed[] embeds = null)
    {
        await _command.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
    }

    public async Task ReactAsync(IEmote emote, RequestOptions options = null)
    {
        await _command.ModifyOriginalResponseAsync(x =>
        {
            x.Content = emote.Name;
        }, options);
    }

    public void Dispose()
    {
    }
}