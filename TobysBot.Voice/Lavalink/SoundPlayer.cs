using Discord;
using TobysBot.Voice.Status;
using Victoria;
using Victoria.Enums;

namespace TobysBot.Voice.Lavalink;

public class SoundPlayer : LavaPlayer
{
    public SoundPlayer(LavaSocket lavaSocket, IVoiceChannel voiceChannel, ITextChannel textChannel) : base(lavaSocket, voiceChannel, textChannel)
    {
    }

    public ISound Sound => new LavaSound(Track);

    public IPlayerStatus Status
    {
        get
        {
            return PlayerState switch
            {
                PlayerState.Stopped => new NotPlayingStatus(VoiceChannel, TextChannel),
                PlayerState.Playing or PlayerState.Paused => new PlayingStatus(VoiceChannel, TextChannel,
                    Sound, Track.Position, PlayerState is PlayerState.Paused),
                _ => new NotConnectedStatus()
            };
        }
    }
}