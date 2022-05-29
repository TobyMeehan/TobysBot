using Discord;

namespace TobysBot.Music;

public interface IMusicService
{
    // Player 
    
    Task<ITrack> EnqueueAsync(IGuild guild, params ITrack[] tracks);
    Task<ITrack> EnqueueAsync(IGuild guild, IEnumerable<ITrack> t);

    Task PauseAsync(IGuild guild);

    Task ResumeAsync(IGuild guild);

    Task SeekAsync(IGuild guild, TimeSpan timeSpan);
    
    // Queue
    
    Task<ITrack> SkipAsync(IGuild guild);
    Task<ITrack> BackAsync(IGuild guild);
    Task<ITrack> JumpAsync(IGuild guild, int track);

    Task ClearAsync(IGuild guild);

    Task StopAsync(IGuild guild);

    Task RemoveAsync(IGuild guild, int track);
    Task RemoveRangeAsync(IGuild guild, int startTrack, int endTrack);

    Task MoveAsync(IGuild guild, int track, int newPos);

    Task SetLoopAsync(IGuild guild, ILoopSetting setting);
    Task SetShuffleAsync(IGuild guild, bool shuffle);

    Task<IQueue> GetQueueAsync(IGuild guild);
}