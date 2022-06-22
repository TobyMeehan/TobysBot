using TobysBot.Events;
using TobysBot.Music.Extensions;
using TobysBot.Voice;
using TobysBot.Voice.Events;

namespace TobysBot.Music.Events;

public class TrackStuckEventHandler : IEventHandler<SoundStuckEventArgs>
{
    private readonly IMusicService _music;
    private readonly IVoiceService _voice;

    public TrackStuckEventHandler(IMusicService music, IVoiceService voice)
    {
        _music = music;
        _voice = voice;
    }
    
    public async Task HandleAsync(SoundStuckEventArgs args)
    {
        var track = await _music.GetTrackAsync(args.Guild);

        if (track is null)
        {
            return;
        }

        await _voice.PlayAsync(args.Guild, track.ToSound(), track.Position);
    }
}