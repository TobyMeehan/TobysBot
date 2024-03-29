using Discord;
using Discord.WebSocket;

namespace TobysBot.Commands.Response;

public class SocketSlashCommandResponseService : ISocketResponseService
{
    private readonly SocketSlashCommand _command;

    public SocketSlashCommandResponseService(SocketSlashCommand command)
    {
        _command = command;
    }
    
    public async Task<ISocketResponse> ReplyAsync(string? text = null, bool isTTS = false, Visibility visibility = Visibility.Public, Embed? embed = null,
        AllowedMentions? allowedMentions = null, RequestOptions? options = null, MessageComponent? components = null,
        ISticker[]? stickers = null, Embed[]? embeds = null)
    {
        await _command.RespondAsync(text, embeds, isTTS, visibility is not Visibility.Public, allowedMentions, components, embed, options);

        return new SocketSlashCommandResponse(_command);
    }

    public async Task<ISocketResponse> DeferAsync(bool ephemeral = false, RequestOptions? options = null)
    {
        await _command.DeferAsync(ephemeral, options);

        return new SocketSlashCommandResponse(_command);
    }

    public async Task ReactAsync(IEmote emote, Visibility visibility = Visibility.Public, RequestOptions? options = null)
    {
        await _command.RespondAsync($"{emote.Name}\u2800", ephemeral: visibility is not Visibility.Public, options: options);
    }
}