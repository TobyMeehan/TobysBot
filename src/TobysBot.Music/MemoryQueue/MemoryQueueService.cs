using System.Collections.Concurrent;

namespace TobysBot.Music.MemoryQueue;

public class MemoryQueueService : IMemoryQueueService
{
    private readonly ConcurrentDictionary<ulong, TrackCollection> _queues = new();

    public TrackCollection GetOrAdd(ulong guild) => _queues.GetOrAdd(guild, _ => new TrackCollection());
    
    public TrackCollection this[ulong guild]
    {
        get => _queues.TryGetValue(guild, out var tracks) ? tracks : new TrackCollection();
        set => _queues[guild] = value;
    }
}