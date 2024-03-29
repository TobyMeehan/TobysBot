using Discord;
using TobysBot.Music.Extensions;
using TobysBot.Voice;
using TobysBot.Voice.Status;

namespace TobysBot.Music.MemoryQueue;

public class MemoryMusicService : IMusicService
{
    private readonly IVoiceService _voice;
    private readonly IMemoryQueueService _queues;

    public MemoryMusicService(IVoiceService voice, IMemoryQueueService queues)
    {
        _voice = voice;
        _queues = queues;
    }

    private IPlayerStatus ThrowIfNotConnected(IGuild guild)
    {
        var status = _voice.Status(guild);

        if (status is not IConnectedStatus)
        {
            throw new Exception("Not connected to voice channel.");
        }

        return status;
    }
    
    public async Task<ITrack> EnqueueAsync(IGuild guild, params ITrack[] tracks)
    {
        return await EnqueueAsync(guild, tracks.ToList());
    }

    public async Task<ITrack> EnqueueAsync(IGuild guild, IEnumerable<ITrack> t)
    {
        var status = _voice.Status(guild);

        var tracks = t.ToList();

        bool isStopped = status is not PlayingStatus;
        
        if (isStopped)
        {
            await _voice.PlayAsync(guild, tracks.First().ToSound(), TimeSpan.Zero);
        }

        var queue = _queues.GetOrAdd(guild.Id);
        
        queue.AddRange(tracks, advanceToTracks: isStopped);

        if (queue.CurrentTrack is null)
        {
            throw new NullReferenceException("Current track is null.");
        }
        
        return queue.CurrentTrack.InnerTrack;
    }

    public async Task<ITrack> EnqueueAsync(IGuild guild, ISavedQueue savedQueue)
    {
        return await EnqueueAsync(guild, savedQueue.Tracks);
    }

    public async Task PauseAsync(IGuild guild)
    {
        ThrowIfNotConnected(guild);

        await _voice.PauseAsync(guild);
    }

    public async Task ResumeAsync(IGuild guild)
    {
        var status = ThrowIfNotConnected(guild);

        if (status is PlayingStatus {IsPaused: true})
        {
            await _voice.ResumeAsync(guild);
            return;
        }

        if (status is not PlayingStatus)
        {
            var track = _queues[guild.Id].CurrentTrack;

            if (track is null)
            {
                return;
            }

            await _voice.PlayAsync(guild, track.ToSound(), TimeSpan.Zero);
        }
    }

    public async Task SeekAsync(IGuild guild, TimeSpan timeSpan)
    {
        ThrowIfNotConnected(guild);

        await _voice.SeekAsync(guild, timeSpan);
    }

    public async Task<ITrack?> SkipAsync(IGuild guild)
    {
        ThrowIfNotConnected(guild);
        
        var nextTrack = _queues[guild.Id].Advance(true);

        if (nextTrack is null)
        {
            await _voice.StopAsync(guild);
        }
        else
        {
            await _voice.PlayAsync(guild, nextTrack.ToSound(), TimeSpan.Zero);
        }

        return nextTrack;
    }

    public async Task<ITrack> BackAsync(IGuild guild)
    {
        ThrowIfNotConnected(guild);

        var previousTrack = _queues[guild.Id].Back();

        if (previousTrack is null)
        {
            throw new Exception("No previous track to play.");
        }

        await _voice.PlayAsync(guild, previousTrack.ToSound(), TimeSpan.Zero);

        return previousTrack;
    }

    public async Task<ITrack> JumpAsync(IGuild guild, int index)
    {
        ThrowIfNotConnected(guild);

        if (index < 1)
        {
            throw new ArgumentException("Index must be >= 1.", nameof(index));
        }

        var track = _queues[guild.Id].Jump(index - 1);

        if (track is null)
        {
            throw new Exception("Invalid track or index.");
        }

        await _voice.PlayAsync(guild, track.ToSound(), TimeSpan.Zero);

        return track;
    }

    public async Task ClearAsync(IGuild guild)
    {
        ThrowIfNotConnected(guild);
        
        _queues[guild.Id].Clear();

        await _voice.StopAsync(guild);
    }

    public async Task StopAsync(IGuild guild)
    {
        ThrowIfNotConnected(guild);
        
        _queues[guild.Id].Stop();

        await _voice.StopAsync(guild);
    }

    public async Task RemoveAsync(IGuild guild, int track)
    {
        ThrowIfNotConnected(guild);

        var queue = _queues[guild.Id];

        if (queue.Remove(track - 1))
        {
            if (queue.CurrentTrack is null)
            {
                return;
            }
            
            await _voice.PlayAsync(guild, queue.CurrentTrack.ToSound(), TimeSpan.Zero);
        }
    }

    public async Task RemoveRangeAsync(IGuild guild, int startTrack, int endTrack)
    {
        ThrowIfNotConnected(guild);

        var queue = _queues[guild.Id];

        if (queue.RemoveRange(startTrack - 1, endTrack - 1))
        {
            if (queue.CurrentTrack is null)
            {
                return;
            }
            
            await _voice.PlayAsync(guild, queue.CurrentTrack.ToSound(), TimeSpan.Zero);
        }
    }

    public Task MoveAsync(IGuild guild, int track, int newPos)
    {
        ThrowIfNotConnected(guild);

        _queues[guild.Id].Move(track - 1, newPos - 1);
        
        return Task.CompletedTask;
    }

    public Task SetLoopAsync(IGuild guild, ILoopSetting setting)
    {
        _queues[guild.Id].LoopEnabled = setting;
        
        return Task.CompletedTask;
    }

    public Task SetShuffleAsync(IGuild guild, bool shuffle)
    {
        _queues[guild.Id].Shuffle = shuffle;
        
        return Task.CompletedTask;
    }

    public Task<IQueue> GetQueueAsync(IGuild guild)
    {
        return Task.FromResult<IQueue>(new Queue(_queues[guild.Id]));
    }
}