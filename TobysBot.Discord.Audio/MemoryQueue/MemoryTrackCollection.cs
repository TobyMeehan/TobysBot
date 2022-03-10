using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TobysBot.Discord.Audio.MemoryQueue
{
    public class MemoryTrackCollection : IQueueStatus
    {
        private readonly Random _rng = new();
        
        private List<MemoryTrack> _tracks => _played.Append(_currentTrack).Concat(_queue).ToList();
        private List<MemoryTrack> _played = new();
        private List<MemoryTrack> _queue = new();
        private MemoryTrack _currentTrack;

        public IEnumerable<ITrack> Previous()
        {
            return _played;
        }

        public IEnumerable<ITrack> Next()
        {
            return _queue;
        }

        public ITrack CurrentTrack => _currentTrack;

        public LoopSetting LoopEnabled { get; set; } = new DisabledLoopSetting();

        public ShuffleSetting ShuffleEnabled { get; set; } = new DisabledShuffleSetting();

        public ITrack Advance(int index)
        {
            if (index != -1)
            {
                if (index < 0 && index >= _tracks.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                
                var previous = _tracks.Take(index);
                var current = _tracks[index];
                var next = _tracks.Skip(index + 1);

                _played = previous.ToList();
                _currentTrack = current;
                _queue = next.ToList();

                return _currentTrack;
            }
            
            if (LoopEnabled is TrackLoopSetting)
            {
                return _currentTrack;
            }

            if (!_queue.Any())
            {
                if (LoopEnabled is QueueLoopSetting)
                {
                    _currentTrack = _played.First();
                    
                    _queue.AddRange(_played.Skip(1));

                    _played.Clear();
                    
                    return _currentTrack;
                }
                
                _played.Add(_currentTrack);
                _currentTrack = null;
                return null;
            }

            if (ShuffleEnabled is EnabledShuffleSetting)
            {
                int element = _rng.Next(0, _queue.Count);
                _played.Add(_currentTrack);
                _currentTrack = _queue[element];
                _queue.RemoveAt(element);

                return _currentTrack;
            }
            
            _played.Add(_currentTrack);
            _currentTrack = _queue.First();
            _queue.RemoveAt(0);

            return _currentTrack;
        }

        public void AddRange(IEnumerable<ITrack> tracks, bool advanceToTracks = false)
        {
            if (advanceToTracks)
            {
                _played = _tracks.ToList();
                _currentTrack = null;
                _queue.Clear();
            }

            _queue.AddRange(
                from track in tracks
                select new MemoryTrack(track));

            if (_currentTrack is null)
            {
                _currentTrack = _queue.First();
                _queue.RemoveAt(0);
            }
        }

        public void Reset()
        {
            _queue = _tracks;
            _currentTrack = _queue.First();
            _queue.RemoveAt(0);
        }

        public void Shuffle()
        {
            int n = _queue.Count;
            
            while (n > 1) {  
                n--;  
                int k = _rng.Next(n + 1);
                (_queue[k], _queue[n]) = (_queue[n], _queue[k]);
            }
        }
        
        public IEnumerator<ITrack> GetEnumerator()
        {
            return _tracks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}