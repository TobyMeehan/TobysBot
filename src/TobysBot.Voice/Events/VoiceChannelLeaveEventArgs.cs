using Discord;

namespace TobysBot.Voice.Events;

public class VoiceChannelLeaveEventArgs
{
    public VoiceChannelLeaveEventArgs(IVoiceChannel channel, IGuild guild)
    {
        Channel = channel;
        Guild = guild;
    }

    public IVoiceChannel Channel { get; }
    public IGuild Guild { get; }
}