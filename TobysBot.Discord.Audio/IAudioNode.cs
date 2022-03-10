using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TobysBot.Discord.Audio
{
    public interface IAudioNode
    {
        // Voice channel

        /// <summary>
        /// Connects or moves a player to the specified voice channel.
        /// </summary>
        /// <param name="channel">Voice channel to connect to.</param>
        /// <param name="textChannel">Text channel for event messages.</param>
        /// <returns></returns>
        Task JoinAsync(IVoiceChannel channel, ITextChannel textChannel = null);

        /// <summary>
        /// Disconnects the player (if one exists) from the specified guild.
        /// </summary>
        /// <param name="guild">Guild to attempt to disconnect.</param>
        /// <returns></returns>
        Task LeaveAsync(IGuild guild);

        // Player

        /// <summary>
        /// Adds the specified track(s) to the queue, and begins playback if the queue is empty. Returns the track currently playing.
        /// </summary>
        /// <param name="source">Track or playlist to add the queue.</param>
        /// <param name="guild">Guild in which to play track.</param>
        /// <returns></returns>
        Task<ITrack> EnqueueAsync(IPlayable source, IGuild guild);

        /// <summary>
        /// Pauses player in the specified guild.
        /// </summary>
        /// <param name="guild">Guild in which to pause player.</param>
        /// <returns></returns>
        Task PauseAsync(IGuild guild);

        /// <summary>
        /// Resumes a paused player in the specified guild.
        /// </summary>
        /// <param name="guild">Guild in which to resume player.</param>
        /// <returns></returns>
        Task ResumeAsync(IGuild guild);

        /// <summary>
        /// Moves the player to the specified timespan.
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        Task SeekAsync(IGuild guild, TimeSpan timeSpan);

        // Queue

        /// <summary>
        /// Skips to the next track in the queue in the specified guild.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task<ITrack> SkipAsync(IGuild guild, int index = 0);

        /// <summary>
        /// Clears the queue and stops player in the specified guild.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task ClearAsync(IGuild guild);

        /// <summary>
        /// Stops player and returns to start of the queue in the specified guild.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task StopAsync(IGuild guild);

        // Status

        IPlayerStatus Status(IGuild guild);
        
        /// <summary>
        /// Gets the voice channel the player for the specified guild is connected to.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task<IVoiceChannel> GetCurrentChannelAsync(IGuild guild);

        /// <summary>
        /// Gets the current track for the specified guild.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task<ITrack> GetCurrentTrackAsync(IGuild guild);

        /// <summary>
        /// Gets the queue for the specified guild.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task<IQueueStatus> GetQueueAsync(IGuild guild);

        Task SetLoopAsync(IGuild guild, LoopSetting setting);

        /// <summary>
        /// Enables shuffle mode for the queue.
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        Task SetShuffleAsync(IGuild guild, ShuffleSetting setting);

        /// <summary>
        /// Performs a one-time shuffle on the queue.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task ShuffleAsync(IGuild guild);
    }
}
