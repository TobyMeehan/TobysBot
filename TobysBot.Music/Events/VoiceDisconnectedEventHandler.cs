using TobysBot.Events;
using TobysBot.Music.MemoryQueue;
using TobysBot.Voice;
using TobysBot.Voice.Events;

namespace TobysBot.Music.Events;

public class VoiceDisconnectedEventHandler : IEventHandler<VoiceChannelLeaveEventArgs>
{
    private readonly IMemoryQueueService _queues;

    public VoiceDisconnectedEventHandler(IMemoryQueueService queues)
    {
        _queues = queues;
    }
    
    public Task HandleAsync(VoiceChannelLeaveEventArgs args)
    {
        _queues[args.Guild.Id].Clear();
        return Task.CompletedTask; 
    }
}