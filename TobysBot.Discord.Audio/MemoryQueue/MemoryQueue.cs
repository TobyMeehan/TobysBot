using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace TobysBot.Discord.Audio.MemoryQueue
{
    public class MemoryQueue : IQueue
    {
        private readonly ConcurrentDictionary<ulong, MemoryTrackCollection> _queue = new();

        public Task EnqueueAsync(ulong guild, IEnumerable<ITrack> tracks, bool advanceToTracks = false)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                queue = new MemoryTrackCollection();
                _queue[guild] = queue;
            }
            
            queue.AddRange(tracks, advanceToTracks);
            
            return Task.CompletedTask;
        }

        public Task<(bool TrackChanged, ITrack CurrentTrack)> RemoveAsync(ulong guildId, int index)
        {
            if (!_queue.TryGetValue(guildId, out var queue))
            {
                throw new Exception("No queue for specified guild.");
            }
            
            var changed = queue.Remove(index-1);
            
            return Task.FromResult<(bool TrackChanged, ITrack CurrentTrack)>((changed, queue.CurrentTrack));
        }

        public Task<(bool TrackChanged, ITrack CurrentTrack)> RemoveRangeAsync(ulong guildId, int startIndex, int endIndex)
        {
            if (!_queue.TryGetValue(guildId, out var queue))
            {
                throw new Exception("No queue for specified guild.");
            }
            
            var changed = queue.RemoveRange(startIndex-1, endIndex-1);
            
            return Task.FromResult<(bool TrackChanged, ITrack CurrentTrack)>((changed, queue.CurrentTrack));
        }

        public Task<(bool TrackChanged, ITrack CurrentTrack)> MoveAsync(ulong guildId, int index, int destIndex)
        {
            if (!_queue.TryGetValue(guildId, out var queue))
            {
                throw new Exception("No queue for specified guild.");
            }
            
            var changed = queue.Move(index-1, destIndex-1);

            return Task.FromResult<(bool TrackChanged, ITrack CurrentTrack)>((changed, queue.CurrentTrack));
        }

        public Task<IQueueStatus> GetAsync(ulong guild)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.FromResult<IQueueStatus>(null);
            }

            return Task.FromResult<IQueueStatus>(new MemoryQueueStatus(queue));
        }
        
        public Task<ITrack> AdvanceAsync(ulong guild, bool ignoreTrackLoop = false)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.FromResult<ITrack>(null);
            }

            return Task.FromResult(queue.Advance(ignoreTrackLoop));
        }

        public Task<ITrack> JumpAsync(ulong guild, int index)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.FromResult<ITrack>(null);
            }
            
            return Task.FromResult(queue.Jump(index-1));
        }

        public Task<ITrack> BackAsync(ulong guild)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.FromResult<ITrack>(null);
            }

            return Task.FromResult(queue.Back());
        }

        public Task ProgressAsync(ulong guild, TimeSpan position)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.CompletedTask;
            }
            
            queue.Progress(position);
            
            return Task.CompletedTask;
        }

        public Task ClearAsync(ulong guild)
        {
            if (!_queue.TryRemove(guild, out var value))
            {
                return Task.FromException(new Exception("Failed to clear queue."));
            }

            return Task.CompletedTask;
        }

        public Task ResetAsync(ulong guild)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.CompletedTask;
            }
            
            queue.Reset();
            
            return Task.CompletedTask;
        }

        public Task SetLoopAsync(ulong guild, LoopSetting setting)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.CompletedTask;
            }

            queue.LoopEnabled = setting;

            return Task.CompletedTask;
        }

        public Task SetShuffleAsync(ulong guild, ShuffleSetting setting)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.CompletedTask;
            }

            queue.ShuffleEnabled = setting;
            
            return Task.CompletedTask;
        }

        public Task ShuffleAsync(ulong guild)
        {
            if (!_queue.TryGetValue(guild, out var queue))
            {
                return Task.CompletedTask;
            }
            
            queue.Shuffle();
            
            return Task.CompletedTask;
        }
    }
}