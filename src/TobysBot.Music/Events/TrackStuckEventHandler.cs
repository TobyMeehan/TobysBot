using TobysBot.Events;
using TobysBot.Music.Extensions;
using TobysBot.Voice;
using TobysBot.Voice.Events;

namespace TobysBot.Music.Events;

public class TrackStuckEventHandler : IEventHandler<SoundStuckEventArgs>
{
    private readonly IMusicService _music;
    private readonly IVoiceService _voice;
    private readonly IAudioService _audio;

    public TrackStuckEventHandler(IMusicService music, IVoiceService voice, IAudioService audio)
    {
        _music = music;
        _voice = voice;
        _audio = audio;
    }
    
    public async Task HandleAsync(SoundStuckEventArgs args)
    {
        var track = await _music.GetTrackAsync(args.Guild);

        if (track is null)
        {
            return;
        }

        await _voice.PlayAsync(args.Guild, await _audio.LoadAudioAsync(track), track.Position);
    }
}