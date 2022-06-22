using Discord;
using TobysBot.Voice.Status;

namespace TobysBot.Voice.Events;

public class SoundEndedEventArgs
{
    public IGuild Guild { get; }
    public ISound Sound { get; }
    public IPlayerStatus Status { get; }
    public SoundEndedReason Reason { get; }

    public SoundEndedEventArgs(IGuild guild, ISound sound, IPlayerStatus status, SoundEndedReason reason)
    {
        Guild = guild;
        Sound = sound;
        Status = status;
        Reason = reason;
    }
}

public enum SoundEndedReason : byte
{
    Finished = (byte) 'F',
    LoadFailed = (byte) 'L',
    Stopped = (byte) 'S',
    Replaced = (byte) 'R',
    Cleanup = (byte) 'C'
}