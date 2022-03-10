﻿using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TobysBot.Discord.Audio
{
    public interface IQueue
    {
        /// <summary>
        /// Adds the specified track or playlist to the queue for the specified guild.
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="tracks"></param>
        /// <returns></returns>
        Task EnqueueAsync(IGuild guild, IEnumerable<ITrack> tracks, bool advanceToTracks = false);

        /// <summary>
        /// Gets the queue for the specified guild.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task<IQueueStatus> GetAsync(IGuild guild);

        /// <summary>
        /// Advances to the next track in the queue and returns the current track.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task<ITrack> AdvanceAsync(IGuild guild, int index = 0);

        /// <summary>
        /// Clears the queue for the specified guild.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task ClearAsync(IGuild guild);

        /// <summary>
        /// Returns to the start of the queue.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task ResetAsync(IGuild guild);

        Task SetLoopAsync(IGuild guild, LoopSetting setting);

        Task SetShuffleAsync(IGuild guild, ShuffleSetting setting);

        Task ShuffleAsync(IGuild guild);
    }
}
