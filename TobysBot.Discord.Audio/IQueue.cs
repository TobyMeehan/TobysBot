using Discord;
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
        /// Gets the queue for the specified guild.
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        Task<IQueueStatus> GetAsync(ulong guildId);

        /// <summary>
        /// Advances to the next track in the queue and returns the current track.
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        Task<ITrack> AdvanceAsync(ulong guildId, int index = 0);

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
