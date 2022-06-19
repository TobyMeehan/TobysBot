namespace TobysBot.Music.MemoryQueue;

public interface IMemoryQueueService
{
    TrackCollection GetOrAdd(ulong guild);
    TrackCollection this[ulong guild] { get; set; }
}