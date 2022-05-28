using Discord;
using Victoria;

namespace TobysBot.Voice.Lavalink;

public class SoundPlayer : LavaPlayer
{
    public SoundPlayer(LavaSocket lavaSocket, IVoiceChannel voiceChannel, ITextChannel textChannel) : base(lavaSocket, voiceChannel, textChannel)
    {
    }

    public ISound Sound => new LavaSound(Track);
}