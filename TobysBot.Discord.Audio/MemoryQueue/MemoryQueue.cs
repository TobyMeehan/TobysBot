using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace TobysBot.Discord.Audio.MemoryQueue
{
    public class MemoryQueue : IQueue
    {
        private readonly Dictionary<IGuild, MemoryTrackCollection> _queue = new Dictionary<IGuild, MemoryTrackCollection>();

        public Task EnqueueAsync(IGuild guild, IEnumerable<ITrack> tracks, bool advanceToTracks = false)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                queue = new MemoryTrackCollection();
                _queue.Add(guild, queue);
            }
            
            queue.AddRange(tracks, advanceToTracks);
            
            return Task.CompletedTask;
        }

        public Task<IQueueStatus> GetAsync(IGuild guild)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.FromResult<IQueueStatus>(null);
            }

            return Task.FromResult<IQueueStatus>(queue);
        }

        public Task<ITrack> AdvanceAsync(IGuild guild)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return null;
            }
            
            return Task.FromResult<ITrack>(queue.Advance());
        }

        public Task<ITrack> PeekAsync(IGuild guild)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return null;
            }
            
            return Task.FromResult<ITrack>(queue.NextTrack);
        }

        public Task ClearAsync(IGuild guild)
        {
            _queue.Remove(guild);

            return Task.CompletedTask;
        }

        public Task ResetAsync(IGuild guild)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.CompletedTask;
            }
            
            queue.Reset();
            
            return Task.CompletedTask;
        }

        public Task SetLoopAsync(IGuild guild, LoopSetting setting)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.CompletedTask;
            }

            queue.LoopEnabled = setting;

            return Task.CompletedTask;
        }
    }
}