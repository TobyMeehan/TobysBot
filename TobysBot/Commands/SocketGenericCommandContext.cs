using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TobysBot.Commands.Response;

namespace TobysBot.Commands;

public class SocketGenericCommandContext : ICommandContext
{
    public SocketGenericCommandContext(DiscordSocketClient client, SocketUserMessage message)
    {
        Client = client;
        Guild = (message.Channel as SocketGuildChannel)?.Guild;
        Channel = message.Channel;
        User = message.Author;

        Message = message;
    }

    public SocketGenericCommandContext(DiscordSocketClient client, SocketSlashCommand command)
    {
        Client = client;
        Guild = (command.Channel as SocketGuildChannel)?.Guild;
        Channel = command.Channel;
        User = command.User;

        Message = null;
        Response = new SocketSlashCommandResponseService(command);
    }

    public ISocketResponseService Response { get; }
    
    public DiscordSocketClient Client { get; }
    public SocketGuild Guild { get; }
    public ISocketMessageChannel Channel { get; }
    public SocketUser User { get; }

    IDiscordClient ICommandContext.Client => Client;
    IGuild ICommandContext.Guild => Guild;
    IMessageChannel ICommandContext.Channel => Channel;
    IUser ICommandContext.User => User;
    
    public IUserMessage Message { get; }
}