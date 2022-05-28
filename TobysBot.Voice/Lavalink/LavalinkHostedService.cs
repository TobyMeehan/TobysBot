using Microsoft.Extensions.Hosting;
using TobysBot.Events;
using TobysBot.Voice.Events;
using Victoria;
using Victoria.EventArgs;

namespace TobysBot.Voice.Lavalink;

public class LavalinkHostedService : IHostedService
{
    private readonly LavaNode _lavaNode;
    private readonly IEventService _events;

    public LavalinkHostedService(LavaNode lavaNode, IEventService events)
    {
        _lavaNode = lavaNode;
        _events = events;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _lavaNode.ConnectAsync();
        
        SubscribeEvents();
    }

    public void SubscribeEvents()
    {
        _lavaNode.OnLog += (message) => _events.InvokeAsync(new LavalinkLogEventArgs(message));
        _lavaNode.OnTrackEnded += OnTrackEnded;
        _lavaNode.OnTrackException += OnTrackException;
        _lavaNode.OnTrackStarted += OnTrackStarted;
        _lavaNode.OnTrackStuck += OnTrackStuck;
    }

    private Task OnTrackStuck(TrackStuckEventArgs arg)
    {
        if (arg.Player is not SoundPlayer player)
        {
            return Task.CompletedTask;
        }

        return _events.InvokeAsync(new SoundStuckEventArgs(player.Sound, player.Status, arg.Threshold));
    }

    private Task OnTrackStarted(TrackStartEventArgs arg)
    {
        if (arg.Player is not SoundPlayer player)
        {
            return Task.CompletedTask;
        }

        return _events.InvokeAsync(new SoundStartedEventArgs(player.Sound, player.Status));
    }

    private Task OnTrackException(TrackExceptionEventArgs arg)
    {
        if (arg.Player is not SoundPlayer player)
        {
            return Task.CompletedTask;
        }

        return _events.InvokeAsync(new SoundExceptionEventArgs(player.Sound, player.Status, arg.Exception.Message));
    }

    private Task OnTrackEnded(TrackEndedEventArgs arg)
    {
        if (arg.Player is not SoundPlayer player)
        {
            return Task.CompletedTask;
        }

        return _events.InvokeAsync(new SoundEndedEventArgs(player.Sound, player.Status, (SoundEndedReason)(byte) arg.Reason));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _lavaNode.DisconnectAsync();
    }
}