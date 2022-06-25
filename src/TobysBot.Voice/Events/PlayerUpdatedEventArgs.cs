using Discord;
using TobysBot.Voice.Status;

namespace TobysBot.Voice.Events;

public class PlayerUpdatedEventArgs
{
    public PlayerUpdatedEventArgs(IPlayerStatus status, TimeSpan? position, IGuild guild)
    {
        Status = status;
        Position = position;
        Guild = guild;
    }

    public IPlayerStatus Status { get; }
    public IGuild Guild { get; }
    public TimeSpan? Position { get; }
}