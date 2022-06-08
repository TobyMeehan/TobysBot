using TobysBot.Events;
using TobysBot.Music.MemoryQueue;
using TobysBot.Voice.Events;

namespace TobysBot.Music.Events;

public class TrackProgressEventHandler : IEventHandler<PlayerUpdatedEventArgs>
{
    private readonly IMemoryQueueService _queues;

    public TrackProgressEventHandler(IMemoryQueueService queues)
    {
        _queues = queues;
    }
    
    public Task HandleAsync(PlayerUpdatedEventArgs args)
    {
        if (args.Position.HasValue)
        {
            _queues[args.Guild.Id].Progress(args.Position.Value);
        }
        
        return Task.CompletedTask;
    }
}