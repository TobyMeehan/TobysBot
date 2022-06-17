using Discord;

namespace TobysBot.Events;

public class DiscordClientLogEventArgs
{
    public DiscordClientLogEventArgs(LogMessage message)
    {
        Message = message;
    }

    public LogMessage Message { get; }
}