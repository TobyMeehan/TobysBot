using Microsoft.Extensions.Hosting;
using TobysBot.Events;
using TobysBot.Voice.Events;
using Victoria;
using Victoria.EventArgs;

namespace TobysBot.Voice.Lavalink;

public class LavalinkHostedService : IHostedService, IEventHandler<DiscordClientReadyEventArgs>
{
    private readonly LavaNode<SoundPlayer> _lavaNode;
    private readonly IEventService _events;

    public LavalinkHostedService(LavaNode<SoundPlayer> lavaNode, IEventService events)
    {
        _lavaNode = lavaNode;
        _events = events;
    }
    
    async Task IEventHandler<DiscordClientReadyEventArgs>.HandleAsync(DiscordClientReadyEventArgs args)
    {
        try
        {
            await _lavaNode.ConnectAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        SubscribeEvents();
        
        return Task.CompletedTask;;
    }

    public void SubscribeEvents()
    {
        _lavaNode.OnLog += (message) => _events.InvokeAsync(new LavalinkLogEventArgs(message));
        _lavaNode.OnTrackEnded += OnTrackEnded;
        _lavaNode.OnTrackException += OnTrackException;
        _lavaNode.OnTrackStarted += OnTrackStarted;
        _lavaNode.OnTrackStuck += OnTrackStuck;
        _lavaNode.OnPlayerUpdated += OnPlayerUpdated;
    }

    private Task OnPlayerUpdated(PlayerUpdateEventArgs arg)
    {
        if (arg.Player is not SoundPlayer player)
        {
            return Task.CompletedTask;;
        }

        return _events.InvokeAsync(new PlayerUpdatedEventArgs(player.Status, arg.Position, player.VoiceChannel.Guild));
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