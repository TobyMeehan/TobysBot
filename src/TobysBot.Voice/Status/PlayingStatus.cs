using Discord;

namespace TobysBot.Voice.Status;

public class PlayingStatus : IConnectedStatus
{
    public PlayingStatus(IVoiceChannel voiceChannel, ITextChannel textChannel, ISound sound, TimeSpan position, bool isPaused)
    {
        VoiceChannel = voiceChannel;
        TextChannel = textChannel;
        Sound = sound;
        Position = position;
        IsPaused = isPaused;
    }

    public IVoiceChannel VoiceChannel { get; }
    public ITextChannel TextChannel { get; }

    public ISound Sound { get; }
    public TimeSpan Position { get; }
    public bool IsPaused { get; }
}