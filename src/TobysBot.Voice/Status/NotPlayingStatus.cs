using Discord;

namespace TobysBot.Voice.Status;

public class NotPlayingStatus : IConnectedStatus
{
    public NotPlayingStatus(IVoiceChannel voiceChannel, ITextChannel textChannel)
    {
        VoiceChannel = voiceChannel;
        TextChannel = textChannel;
    }

    public IVoiceChannel VoiceChannel { get; }
    public ITextChannel TextChannel { get; }
}