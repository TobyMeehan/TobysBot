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
            return _tracks.TakeWhile(x => x != CurrentTrack);
        }

        public IEnumerable<ITrack> Next()
        {
            return _tracks.SkipWhile(x => x != CurrentTrack).Skip(1);
        }

        public ITrack CurrentTrack => _tracks[_currentIndex];

        public ITrack NextTrack => _currentIndex + 1 >= _tracks.Count ? null : _tracks[_currentIndex + 1];

        public ITrack Advance()
        {
            if (++_currentIndex >= _tracks.Count)
            {
                return null;
            }

            return CurrentTrack;
        }

        public void AddRange(IEnumerable<ITrack> tracks)
        {
            _tracks.AddRange(tracks);
        }

        public void Clear()
        {
            _tracks.Clear();
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