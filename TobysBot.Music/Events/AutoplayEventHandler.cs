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

    public AutoplayEventHandler(IMemoryQueueService queues, IVoiceService voice)
    {
        _queues = queues;
        _voice = voice;
    }
    
    public async Task HandleAsync(SoundEndedEventArgs args)
    {
        if (args.Reason is not SoundEndedReason.Finished)
        {
            return;
        }

        var track = _queues[args.Guild.Id].Advance(false);

        if (track is null)
        {
            return;
        }

        await _voice.PlayAsync(track.ToSound(), args.Guild);
    }
}