using System;
using System.Threading.Tasks;
using Discord;
using Victoria;
using Victoria.Enums;

namespace TobysBot.Discord.Audio.Lavalink;

public class XLavaPlayer : LavaPlayer
{
    public XLavaPlayer(LavaSocket lavaSocket, IVoiceChannel voiceChannel, ITextChannel textChannel) : base(lavaSocket, voiceChannel, textChannel)
    {
    }
    
    public ITrack CurrentTrack => new LavalinkTrack(Track);

    public IPlayerStatus Status
    {
        get
        {
            return PlayerState switch
            {
                PlayerState.Playing => new PlayingStatus(new LavalinkActiveTrack(Track)),
                PlayerState.Paused => new PausedStatus(new LavalinkActiveTrack(Track)),
                _ => new NotPlayingStatus()
            };
        }
    }
}