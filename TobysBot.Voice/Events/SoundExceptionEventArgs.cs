using TobysBot.Voice.Status;

namespace TobysBot.Voice.Events;

public class SoundExceptionEventArgs
{
    public ISound Sound { get; }
    public IPlayerStatus Status { get; }
    public string Exception { get; }

    public SoundExceptionEventArgs(ISound sound, IPlayerStatus status, string exception)
    {
        Sound = sound;
        Status = status;
        Exception = exception;
    }
}