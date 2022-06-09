using System.Collections.Concurrent;
using Discord;
using TobysBot.Events;
using TobysBot.Music.Extensions;
using TobysBot.Voice;
using TobysBot.Voice.Events;
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

        var isStopped = status is not PlayingStatus;
        
        if (isStopped)
        {
            await _voice.PlayAsync(tracks.First().ToSound(), guild);
        }

        _queues.GetOrAdd(guild.Id)
            .AddRange(tracks, advanceToTracks: isStopped);

        return _queues[guild.Id].CurrentTrack.InnerTrack;
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

            await _voice.PlayAsync(track.ToSound(), guild);
        }
    }

    public async Task SeekAsync(IGuild guild, TimeSpan timeSpan)
    {
        ThrowIfNotConnected(guild);

        await _voice.SeekAsync(guild, timeSpan);
    }

    public async Task<ITrack> SkipAsync(IGuild guild)
    {
        ThrowIfNotConnected(guild);
        
        var nextTrack = _queues[guild.Id].Advance(true);

        if (nextTrack is null)
        {
            await _voice.StopAsync(guild);
        }
        else
        {
            await _voice.PlayAsync(nextTrack.ToSound(), guild);
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

        await _voice.PlayAsync(previousTrack.ToSound(), guild);

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

        await _voice.PlayAsync(track.ToSound(), guild);

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

        if (_queues[guild.Id].Remove(track - 1))
        {
            await _voice.PlayAsync(_queues[guild.Id].CurrentTrack.ToSound(), guild);
        }
    }

    public async Task RemoveRangeAsync(IGuild guild, int startTrack, int endTrack)
    {
        ThrowIfNotConnected(guild);

        if (_queues[guild.Id].RemoveRange(startTrack - 1, endTrack - 1))
        {
            await _voice.PlayAsync(_queues[guild.Id].CurrentTrack.ToSound(), guild);
        }
    }

    public async Task MoveAsync(IGuild guild, int track, int newPos)
    {
        ThrowIfNotConnected(guild);

        _queues[guild.Id].Move(track - 1, newPos - 1);
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