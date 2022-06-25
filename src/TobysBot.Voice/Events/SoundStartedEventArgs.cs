using Discord;
using TobysBot.Voice.Status;

namespace TobysBot.Voice.Events;

public class SoundStartedEventArgs
{
    public ISound Sound { get; }
    public IPlayerStatus Status { get; }
    public ITextChannel TextChannel { get; }
    public IGuild Guild { get; }

    public SoundStartedEventArgs(ISound sound, IPlayerStatus status, ITextChannel textChannel, IGuild guild)
    {
        Sound = sound;
        Status = status;
        TextChannel = textChannel;
        Guild = guild;
    }
}