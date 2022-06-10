using Discord;

namespace TobysBot.Events;

public class MessageReceivedEventArgs
{
    public MessageReceivedEventArgs(IMessage message)
    {
        Message = message;
    }

    public IMessage Message { get; }
}