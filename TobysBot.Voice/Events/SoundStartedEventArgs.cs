using TobysBot.Voice.Status;

namespace TobysBot.Voice.Events;

public class SoundStartedEventArgs
{
    public ISound Sound { get; }
    public IPlayerStatus Status { get; }

    public SoundStartedEventArgs(ISound sound, IPlayerStatus status)
    {
        Sound = sound;
        Status = status;
    }
}