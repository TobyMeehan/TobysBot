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
        /// <param name="guild"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        Task EnqueueAsync(IGuild guild, IPlayable source);

        /// <summary>
        /// Gets the queue for the specified guild.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task<IEnumerable<ITrack>> GetAsync(IGuild guild);

        /// <summary>
        /// Removes the first item from the queue and returns it.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task<ITrack> DequeueAsync(IGuild guild);

        /// <summary>
        /// Returns the first item in the queue without removing it.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task<ITrack> PeekAsync(IGuild guild);

        /// <summary>
        /// Clears the queue for the specified guild.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        Task ClearAsync(IGuild guild);
    }
}
