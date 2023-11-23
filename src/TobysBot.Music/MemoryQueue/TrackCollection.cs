using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace TobysBot.Music.MemoryQueue;

public class TrackCollection : IEnumerable<ITrack>
{
    private readonly List<ITrack> _tracks = new();
    private readonly List<int> _played = new();

    private int _currentIndex;
    private TimeSpan _currentPosition = TimeSpan.Zero;
    private bool _paused;
    private bool _stopped = true;
    private Random? _rng;

    public IEnumerable<ITrack> Previous => _tracks.Take(_currentIndex);
    public IEnumerable<ITrack> Next => _tracks.Skip(_currentIndex + 1);
    
    public IActiveTrack? CurrentTrack => _currentIndex < _tracks.Count
        ? new ActiveTrack(_tracks.ElementAt(_currentIndex), _currentPosition, CurrentStatus())
        : null;

    private ActiveTrackStatus CurrentStatus()
    {
        if (_stopped)
        {
            return ActiveTrackStatus.Stopped;
        }

        return _paused ? ActiveTrackStatus.Paused : ActiveTrackStatus.Playing;
    }
    
    public ILoopSetting LoopEnabled { get; set; } = new DisabledLoopSetting();
    
    [MemberNotNullWhen(true, nameof(_rng))]
    public bool Shuffle => _rng is not null;

    public void EnableShuffle(int? seed)
    {
        _rng = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public void DisableShuffle()
    {
        _rng = null;
    }

    public void Update(TimeSpan position, bool isPaused)
    {
        _currentPosition = position;
        _paused = isPaused;
        _stopped = false;
    }

    public ITrack? Advance(bool forceSkip)
    {
        if (!forceSkip && LoopEnabled is TrackLoopSetting)
        {
            return CurrentTrack;
        }

        if (Shuffle && _played.Count == _tracks.Count || !Next.Any())
        {
            if (LoopEnabled is QueueLoopSetting)
            {
                _currentIndex = 0;

                _played.Clear();

                return CurrentTrack;
            }
            
            _currentIndex++;
            _played.Clear();
            return null;
        }
        
        _played.Add(_currentIndex);
        _currentIndex++;

        if (Shuffle)
        {
            var unplayed = _tracks.Where(x => !_played.Contains(_tracks.IndexOf(x))).ToList();

            int index = _rng.Next(0, unplayed.Count);

            var track = unplayed[index];

            _currentIndex = _tracks.IndexOf(track);
        }

        return CurrentTrack;
    }

    public ITrack? Back()
    {
        _currentIndex--;

        return CurrentTrack;
    }
    
    public ITrack? Jump(int index)
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
        bool removed = index == _currentIndex;
        
        if (index < _currentIndex)
        {
            _currentIndex--;
        }
        
        _tracks.RemoveAt(index);

        return removed;
    }

    public bool RemoveRange(int startIndex, int endIndex)
    {
        bool removed = startIndex <= _currentIndex && _currentIndex <= endIndex;

        if (startIndex < _currentIndex && endIndex >= _currentIndex)
        {
            _currentIndex = startIndex;
        }

        int count = endIndex - startIndex + 1;

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