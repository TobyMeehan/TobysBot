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
        /// <param name="guildId"></param>
        /// <param name="tracks"></param>
        /// <returns></returns>
        Task EnqueueAsync(ulong guildId, IEnumerable<ITrack> tracks, bool advanceToTracks = false);

        /// <summary>
        /// Removes the specified track from the queue.
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<(bool TrackChanged, ITrack CurrentTrack)> RemoveAsync(ulong guildId, int index);

        /// <summary>
        /// Removes the specified range of tracks from the queue.
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        Task<(bool TrackChanged, ITrack CurrentTrack)> RemoveRangeAsync(ulong guildId, int startIndex, int endIndex);

        /// <summary>
        /// Moves the specified track to the specified position.
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="index"></param>
        /// <param name="destIndex"></param>
        /// <returns></returns>
        Task<(bool TrackChanged, ITrack CurrentTrack)> MoveAsync(ulong guildId, int index, int destIndex);
        
        /// <summary>
        /// Gets the queue for the specified guild.
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        Task<IQueueStatus> GetAsync(ulong guildId);

        /// <summary>
        /// Advances to the next track in the queue and returns the current track.
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="ignoreTrackLoop"></param>
        /// <returns></returns>
        Task<ITrack> AdvanceAsync(ulong guildId, bool ignoreTrackLoop = false);

        /// <summary>
        /// Advances to the specified track and returns the current track.
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<ITrack> JumpAsync(ulong guild, int index);

        /// <summary>
        /// Advances to the previous track in the queue.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task<ITrack> BackAsync(ulong guild);

        Task ProgressAsync(ulong guildId, TimeSpan position);

        /// <summary>
        /// Clears the queue for the specified guild.
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        Task ClearAsync(ulong guildId);

        /// <summary>
        /// Returns to the start of the queue.
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        Task ResetAsync(ulong guildId);

        Task SetLoopAsync(ulong guildId, LoopSetting setting);

        Task SetShuffleAsync(ulong guildId, ShuffleSetting setting);

        Task ShuffleAsync(ulong guildId);
    }
}
