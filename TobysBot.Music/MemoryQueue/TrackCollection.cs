using System.Collections;

namespace TobysBot.Music.MemoryQueue;

public class TrackCollection : IEnumerable<ITrack>
{
    private List<ITrack> _tracks = new();

    private int _currentIndex;
    private TimeSpan _currentPosition = TimeSpan.Zero;
    private bool _paused;
    private bool _stopped = true;
    private readonly Random _rng = new();

    public IEnumerable<ITrack> Previous => _tracks.Take(_currentIndex);
    public IEnumerable<ITrack> Next => _tracks.Skip(_currentIndex + 1);

    public IActiveTrack CurrentTrack => _currentIndex < _tracks.Count
        ? new ActiveTrack(_tracks.ElementAtOrDefault(_currentIndex), _currentPosition, CurrentStatus())
        : null;

    private ActiveTrackStatus CurrentStatus()
    {
        if (_stopped)
        {
            return ActiveTrackStatus.Stopped;
        }

        if (_paused)
        {
            return ActiveTrackStatus.Paused;
        }

        return ActiveTrackStatus.Playing;
    }
    
    public ILoopSetting LoopEnabled { get; set; } = new DisabledLoopSetting();
    public bool Shuffle { get; set; } = false;

    public void Update(TimeSpan position, bool isPaused)
    {
        _currentPosition = position;
        _paused = isPaused;
        _stopped = false;
    }

    public ITrack Advance(bool forceSkip)
    {
        if (!forceSkip && LoopEnabled is TrackLoopSetting)
        {
            return CurrentTrack;
        }

        if (!Next.Any())
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

        if (Shuffle)
        {
            var nextIndex = _rng.Next(_currentIndex, _tracks.Count);
            var nextTrack = _tracks[nextIndex];
            
            _tracks.RemoveAt(nextIndex);
            _tracks.Insert(_currentIndex, nextTrack);
        }

        return CurrentTrack;
    }

    public ITrack Back()
    {
        _currentIndex--;

        return CurrentTrack;
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

    public void AddRange(IEnumerable<ITrack> tracks, bool advanceToTracks = false)
    {
        if (advanceToTracks)
        {
            _currentIndex = _tracks.Count;
        }
        
        _tracks.AddRange(tracks);
    }

    public bool Remove(int index)
    {
        var removed = index == _currentIndex;
        
        if (index < _currentIndex)
        {
            _currentIndex--;
        }
        
        _tracks.RemoveAt(index);

        return removed;
    }

    public bool RemoveRange(int startIndex, int endIndex)
    {
        var removed = startIndex <= _currentIndex && _currentIndex <= endIndex;

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

        return removed;
    }

    public void Clear()
    {
        _tracks.Clear();
    }

    public void Move(int index, int destIndex)
    {
        if (index < _currentIndex)
        {
            _currentIndex--;
        }

        if (destIndex < _currentIndex)
        {
            _currentIndex++;
        }

        if (index == _currentIndex)
        {
            _currentIndex = destIndex;
        }

        var track = _tracks[index];
        
        _tracks.RemoveAt(index);
        _tracks.Insert(destIndex, track);
    }

    public void Stop()
    {
        _currentIndex = 0;
        _currentPosition = TimeSpan.Zero;
        _stopped = true;
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