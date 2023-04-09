using TobysBot.Commands;
using TobysBot.Events;
using TobysBot.Extensions;
using TobysBot.Music.Extensions;
using TobysBot.Voice;
using TobysBot.Voice.Events;
using TobysBot.Voice.Status;

namespace TobysBot.Music.Events;

public class TrackExceptionEventHandler : IEventHandler<SoundExceptionEventArgs>
{
    private readonly IMusicService _music;
    private readonly IVoiceService _voice;
    private readonly EmbedService _embeds;
    private readonly IAudioService _audio;

    public TrackExceptionEventHandler(IMusicService music, IVoiceService voice, EmbedService embeds, IAudioService audio)
    {
        _music = music;
        _voice = voice;
        _embeds = embeds;
        _audio = audio;
    }
    
    public async Task HandleAsync(SoundExceptionEventArgs args)
    {
        var track = await _music.GetTrackAsync(args.Guild);

        if (track is null)
        {
            return;
        }
        
        if (track.Position < TimeSpan.FromSeconds(1))
        {
            if (args.Status is IConnectedStatus status)
            {
                await status.TextChannel.SendMessageAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription(args.Exception)
                    .Build());
            }
            
            await _music.SkipAsync(args.Guild);
            
            return;
        }

        await _voice.PlayAsync(args.Guild, await _audio.LoadAudioAsync(track), track.Position);
    }
}