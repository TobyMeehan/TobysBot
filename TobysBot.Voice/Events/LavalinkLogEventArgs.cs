using Discord;

namespace TobysBot.Voice.Events;

public class LavalinkLogEventArgs
{
    public LogMessage Message { get; }

    public LavalinkLogEventArgs(LogMessage message)
    {
        Message = message;
    }
}