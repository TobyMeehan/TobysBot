using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TobysBot.Discord.Audio.MemoryQueue
{
    public class MemoryTrackCollection : IQueueStatus
    {
        private readonly List<ITrack> _tracks = new List<ITrack>();
        private int _currentIndex = 0;

        public IEnumerable<ITrack> Previous()
        {
            return _tracks.Take(_currentIndex);
        }

        public IEnumerable<ITrack> Next()
        {
            return _tracks.Skip(_currentIndex + 1);
        }

        public ITrack CurrentTrack => _tracks[_currentIndex];
        
        public bool LoopEnabled { get; set; }

        private int NextIndex()
        {
            if (_currentIndex + 1 >= _tracks.Count)
            {
                if (LoopEnabled)
                {
                    return 0;
                }
            }

            return _currentIndex + 1;
        }
        
        public ITrack NextTrack => NextIndex() >= _tracks.Count ? null : _tracks[NextIndex()];

        public ITrack Advance()
        {
            _currentIndex = NextIndex();
            
            if (_currentIndex >= _tracks.Count)
            {
                return null;
            }
            
            return CurrentTrack;
        }

        public void AddRange(IEnumerable<ITrack> tracks, bool advanceToTracks = false)
        {
            if (advanceToTracks)
            {
                _currentIndex = _tracks.Count;
            }
            
            _tracks.AddRange(tracks);
        }

        public void Clear()
        {
            _tracks.Clear();
            Reset();
        }

        public void Reset()
        {
            _currentIndex = 0;
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