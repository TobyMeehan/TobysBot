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

    private string _title;
    private string _author;
    
    public async Task PlayAsync(LavaTrack track, string title, string author)
    {
        await PlayAsync(track);

        _title = title;
        _author = author;
    }
    
    public ITrack CurrentTrack => new LavalinkTrack(Track, _title, _author);

    public IPlayerStatus Status
    {
        get
        {
            return PlayerState switch
            {
                PlayerState.Playing => new TrackStatus(new LavalinkActiveTrack(Track, _title, _author), VoiceChannel, TextChannel, false),
                PlayerState.Paused => new TrackStatus(new LavalinkActiveTrack(Track, _title, _author), VoiceChannel, TextChannel, true),
                PlayerState.Stopped => new NotPlayingStatus(VoiceChannel, TextChannel),
                PlayerState.None => new NotConnectedStatus(),
                _ => throw new Exception("Unexpected player status.")
            };
        }
    }
}