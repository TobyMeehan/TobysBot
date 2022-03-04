using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkQueue : IQueue
    {
        public Task EnqueueAsync(IGuild guild, IEnumerable<ITrack> tracks)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ITrack>> GetAsync(IGuild guild)
        {
            throw new System.NotImplementedException();
        }

        public Task<ITrack> DequeueAsync(IGuild guild)
        {
            throw new System.NotImplementedException();
        }

        public Task<ITrack> PeekAsync(IGuild guild)
        {
            throw new System.NotImplementedException();
        }

        public Task ClearAsync(IGuild guild)
        {
            throw new System.NotImplementedException();
        }
    }
}