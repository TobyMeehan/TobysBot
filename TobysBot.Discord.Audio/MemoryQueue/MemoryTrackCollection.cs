using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TobysBot.Discord.Audio.MemoryQueue
{
    public class MemoryTrackCollection : IQueueStatus
    {
        private readonly List<MemoryTrack> _tracks = new();
        private int _currentIndex = 0;

        public IEnumerable<ITrack> Previous()
        {
            return _tracks.Take(_currentIndex);
        }

        public IEnumerable<ITrack> Next()
        {
            return _tracks.Skip(_currentIndex + 1);
        }

        public ITrack CurrentTrack
        {
            get
            {
                if (_currentIndex >= 0 && _currentIndex <= _tracks.Count)
                {
                    return _tracks[_currentIndex];
                }

                return null;
            }
        }

        public LoopSetting LoopEnabled { get; set; } = new DisabledLoopSetting();

        private int NextIndex()
        {
            return LoopEnabled switch
            {
                TrackLoopSetting => _currentIndex,
                QueueLoopSetting when _currentIndex + 1 >= _tracks.Count => 0,
                _ => _currentIndex + 1
            };
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
            
            _tracks.AddRange(
                from track in tracks
                select new MemoryTrack(track)
            );
        }

        public void Clear()
        {
            _tracks.Clear();
            Reset();
        }

        public void Reset()
        {
            _currentIndex = 0;
            LoopEnabled = new DisabledLoopSetting();
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