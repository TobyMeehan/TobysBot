using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TobysBot.Discord.Audio.MemoryQueue
{
    public class MemoryTrackCollection : IEnumerable<ITrack>
    {
        private readonly Random _rng = new();

        private int _currentIndex = 0;
        private List<MemoryTrack> _tracks = new();

        public IEnumerable<MemoryTrack> Played => _tracks.Take(_currentIndex);
        public IEnumerable<MemoryTrack> Queue => _tracks.Skip(_currentIndex + 1);

        public MemoryActiveTrack CurrentTrack => _currentIndex < _tracks.Count
            ? new MemoryActiveTrack(_tracks.ElementAtOrDefault(_currentIndex), _currentPosition)
            : null;

        private TimeSpan _currentPosition = TimeSpan.Zero;

        public LoopSetting LoopEnabled { get; set; } = new DisabledLoopSetting();
        public ShuffleSetting ShuffleEnabled { get; set; } = new DisabledShuffleSetting();

        public void Progress(TimeSpan position)
        {
            _currentPosition = position;
        }

        public ITrack Jump(int index)
        {
            if (index < 0 && index >= _tracks.Count)
            {
                throw new IndexOutOfRangeException();
            }

            _currentIndex = index;

            return CurrentTrack;
        }

        public ITrack Back()
        {
            _currentIndex--;

            return CurrentTrack;
        }
        
        public ITrack Advance(bool ignoreTrackLoop)
        {
            if (!ignoreTrackLoop && LoopEnabled is TrackLoopSetting)
            {
                return CurrentTrack;
            }

            if (!Queue.Any())
            {
                if (LoopEnabled is QueueLoopSetting)
                {
                    _currentIndex = 0;
                    
                    return CurrentTrack;
                }

                _currentIndex++;
                return null;
            }

            _currentIndex++;
            
            if (ShuffleEnabled is EnabledShuffleSetting)
            {
                var nextIndex = _rng.Next(_currentIndex, _tracks.Count);
                var nextTrack = _tracks[nextIndex];

                _tracks.RemoveAt(nextIndex);
                _tracks.Insert(_currentIndex, nextTrack);
            }
            
            return CurrentTrack;
        }

        public void AddRange(IEnumerable<ITrack> tracks, bool advanceToTracks = false)
        {
            if (advanceToTracks)
            {
                _currentIndex = _tracks.Count;
            }

            _tracks.AddRange(
                from track in tracks
                select new MemoryTrack(track));
        }

        public bool Remove(int index)
        {
            var track = CurrentTrack;
            
            if (index < _currentIndex)
            {
                _currentIndex--;
            }
            
            _tracks.RemoveAt(index);

            return track.Id != CurrentTrack.Id;
        }

        public bool RemoveRange(int startIndex, int endIndex)
        {
            var track = CurrentTrack;
            
            if (startIndex < _currentIndex && endIndex >= _currentIndex)
            {
                _currentIndex = startIndex;
            }
            
            var count = endIndex - startIndex + 1;

            if (startIndex < _currentIndex && endIndex <= _currentIndex)
            {
                _currentIndex -= count;
            }
            
            _tracks.RemoveRange(startIndex, count);

            return track.Id != CurrentTrack.Id;
        }

        public bool Move(int index, int destIndex)
        {
            var currentTrack = CurrentTrack;
            
            if (index < _currentIndex)
            {
                _currentIndex--;
            }

            if (destIndex <= _currentIndex)
            {
                _currentIndex++;
            }
            
            var track = _tracks[index];
            
            
            _tracks.RemoveAt(index);
            _tracks.Insert(destIndex, track);

            return currentTrack.Id != CurrentTrack.Id;
        }
        
        public void Reset()
        {
            _currentIndex = 0;
        }

        public void Shuffle()
        {
            var queue = Queue.ToList();
            
            int n = queue.Count;
            
            while (n > 1) 
            {  
                n--;  
                int k = _rng.Next(n + 1);
                (queue[k], queue[n]) = (queue[n], queue[k]);
            }

            _tracks = _tracks.Take(_currentIndex + 1).Concat(queue).ToList();
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