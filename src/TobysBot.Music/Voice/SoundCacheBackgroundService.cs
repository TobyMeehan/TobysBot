using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using TobysBot.Events;
using TobysBot.Music.Events;

namespace TobysBot.Music.Voice;

public class SoundCacheBackgroundService : BackgroundService, IEventHandler<TrackAddedEventArgs>
{
    private readonly IAudioService _audio;
    private readonly ConcurrentBag<ITrack> _addedTracks = new();

    public SoundCacheBackgroundService(IAudioService audio)
    {
        _audio = audio;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_addedTracks.Any())
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                continue;
            }

            foreach (var track in _addedTracks)
            {
                await _audio.LoadAudioAsync(track);
            }
            
            _addedTracks.Clear();
        }
    }
    
    Task IEventHandler<TrackAddedEventArgs>.HandleAsync(TrackAddedEventArgs args)
    {
        foreach (var track in args.Tracks)
        {
            _addedTracks.Add(track);
        }
        
        return Task.CompletedTask;
    }
}