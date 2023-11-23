using TobysBot.Events;
using TobysBot.Music.Extensions;
using TobysBot.Music.MemoryQueue;
using TobysBot.Voice;
using TobysBot.Voice.Events;

namespace TobysBot.Music.Events;

public class AutoplayEventHandler : IEventHandler<SoundEndedEventArgs>
{
    private readonly IMemoryQueueService _queues;
    private readonly IVoiceService _voice;
    private readonly IAudioService _audio;

    public AutoplayEventHandler(IMemoryQueueService queues, IVoiceService voice, IAudioService audio)
    {
        _queues = queues;
        _voice = voice;
        _audio = audio;
    }
    
    public async Task HandleAsync(SoundEndedEventArgs args)
    {
        if (args.Reason is not SoundEndedReason.Finished)
        {
            return;
        }

        var queue = _queues[args.Guild.Id];
        
        var track = queue.Advance(false);

        if (track is null)
        {
            return;
        }

        var startTime = queue.LoopEnabled is TrackLoopSetting { Start: not null } loop
            ? loop.Start.Value
            : TimeSpan.Zero;
        
        await _voice.PlayAsync(args.Guild, await _audio.LoadAudioAsync(track), startTime);
    }
}