using System.Collections.Concurrent;

namespace TobysBot.Music.MemoryQueue;

public class ConcurrentQueueDictionary
{
    private readonly ConcurrentDictionary<ulong, TrackCollection> _queues = new();

    public TrackCollection GetOrAdd(ulong guild) => _queues.GetOrAdd(guild, id => new TrackCollection());
    
    public TrackCollection this[ulong guild]
    {
        get => _queues.TryGetValue(guild, out var tracks) ? tracks : new TrackCollection();
        set => _queues[guild] = value;
    }
}