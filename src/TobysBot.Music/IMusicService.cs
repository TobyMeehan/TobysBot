using Discord;

namespace TobysBot.Music;

/// <summary>
/// Service for playing music.
/// </summary>
public interface IMusicService
{
    // Player 
    
    /// <summary>
    /// Adds the specified array of tracks to the queue.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="tracks"></param>
    /// <returns></returns>
    Task<ITrack> EnqueueAsync(IGuild guild, params ITrack[] tracks);
    
    /// <summary>
    /// Adds the specified tracks to the queue.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    Task<ITrack> EnqueueAsync(IGuild guild, IEnumerable<ITrack> t);
    
    /// <summary>
    /// Adds the specified saved queue to the queue.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="savedQueue"></param>
    /// <returns></returns>
    Task<ITrack> EnqueueAsync(IGuild guild, ISavedQueue savedQueue);

    /// <summary>
    /// Pauses the current track.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task PauseAsync(IGuild guild);

    /// <summary>
    /// Resumes the current track.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task ResumeAsync(IGuild guild);

    /// <summary>
    /// Seeks the current track to the specified position.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    Task SeekAsync(IGuild guild, TimeSpan timeSpan);
    
    // Queue
    
    /// <summary>
    /// Skips to the next track in the queue.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task<ITrack?> SkipAsync(IGuild guild);
    
    /// <summary>
    /// Skips to the previous track in the queue.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task<ITrack> BackAsync(IGuild guild);
    
    /// <summary>
    /// Jumps to the track at the specified index in the queue.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="track"></param>
    /// <returns></returns>
    Task<ITrack> JumpAsync(IGuild guild, int track);

    /// <summary>
    /// Clears the queue.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task ClearAsync(IGuild guild);

    /// <summary>
    /// Stops the player and returns to the start of the queue.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task StopAsync(IGuild guild);

    /// <summary>
    /// Removes the specified track from the queue.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="track"></param>
    /// <returns></returns>
    Task RemoveAsync(IGuild guild, int track);
    
    /// <summary>
    /// Removes the specified range of tracks from the queue.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="startTrack"></param>
    /// <param name="endTrack"></param>
    /// <returns></returns>
    Task RemoveRangeAsync(IGuild guild, int startTrack, int endTrack);

    /// <summary>
    /// Moves the specified track to the specified index.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="track"></param>
    /// <param name="newPos"></param>
    /// <returns></returns>
    Task MoveAsync(IGuild guild, int track, int newPos);

    /// <summary>
    /// Sets the queue's loop mode to the specified setting.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="setting"></param>
    /// <returns></returns>
    Task SetLoopAsync(IGuild guild, ILoopSetting setting);

    /// <summary>
    /// Enables shuffle mode with the specified optional random seed.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    Task EnableShuffleAsync(IGuild guild, int? seed);

    /// <summary>
    /// Disables shuffle mode.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task DisableShuffleAsync(IGuild guild);
    
    /// <summary>
    /// Gets the queue for the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task<IQueue> GetQueueAsync(IGuild guild);
}