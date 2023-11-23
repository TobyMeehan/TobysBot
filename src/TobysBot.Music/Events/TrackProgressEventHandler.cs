using TobysBot.Events;
using TobysBot.Music.MemoryQueue;
using TobysBot.Voice.Events;
using TobysBot.Voice.Status;

namespace TobysBot.Music.Events;

public class TrackProgressEventHandler : IEventHandler<PlayerUpdatedEventArgs>
{
    private readonly IMemoryQueueService _queues;
    private readonly IMusicService _music;

    public TrackProgressEventHandler(IMemoryQueueService queues, IMusicService music)
    {
        _queues = queues;
        _music = music;
    }
    
    public async Task HandleAsync(PlayerUpdatedEventArgs args)
    {
        if (!args.Position.HasValue)
        {
            return;
        }

        var queue = _queues[args.Guild.Id];

        var position = args.Position.Value;

        if (queue.LoopEnabled is TrackLoopSetting {End: not null} loop && position > loop.End)
        {
            position = loop.Start ?? TimeSpan.Zero;
            
            await _music.SeekAsync(args.Guild, position);
        }
        
        queue.Update(position, args.Status is PlayingStatus {IsPaused: true});
    }
}