using TobysBot.Voice.Status;

namespace TobysBot.Voice.Events;

public class SoundStuckEventArgs
{
    public ISound Sound { get; }
    public IPlayerStatus Status { get; }
    public TimeSpan Threshold { get; }

    public SoundStuckEventArgs(ISound sound, IPlayerStatus status, TimeSpan threshold)
    {
        Sound = sound;
        Status = status;
        Threshold = threshold;
    }
}