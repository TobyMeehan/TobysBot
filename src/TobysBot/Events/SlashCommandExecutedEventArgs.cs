using Discord.WebSocket;

namespace TobysBot.Events;

public class SlashCommandExecutedEventArgs
{
    public SlashCommandExecutedEventArgs(SocketSlashCommand command)
    {
        Command = command;
    }

    public SocketSlashCommand Command { get; }
}