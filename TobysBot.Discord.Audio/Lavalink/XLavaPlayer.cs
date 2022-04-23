using System;
using System.Threading.Tasks;
using Discord;
using TobysBot.Discord.Audio.Status;
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
                PlayerState.Playing => new TrackStatus(new LavalinkActiveTrack(Track), VoiceChannel, false),
                PlayerState.Paused => new TrackStatus(new LavalinkActiveTrack(Track), VoiceChannel, true),
                PlayerState.Stopped => new NotPlayingStatus(VoiceChannel),
                PlayerState.None => new NotConnectedStatus(),
                _ => throw new Exception("Unexpected player status.")
            };
        }
    }
}