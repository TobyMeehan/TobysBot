using Discord;
using TobysBot.Voice.Status;

namespace TobysBot.Voice.Events;

public class SoundExceptionEventArgs
{
    public IPlayerStatus Status { get; }
    public string Exception { get; }
    public IGuild Guild { get; }

    public SoundExceptionEventArgs(IPlayerStatus status, string exception, IGuild guild)
    {
        Status = status;
        Exception = exception;
        Guild = guild;
    }
}