using Discord;
using TobysBot.Events;
using TobysBot.Voice.Events;
using Victoria;
using Victoria.EventArgs;

namespace TobysBot.Voice.Lavalink.Victoria;

public class VictoriaLavalinkNode : ILavalinkNode
{
    private readonly LavaNode<XLavaPlayer> _node;
    private readonly IEventService _events;

    public VictoriaLavalinkNode(LavaNode<XLavaPlayer> node, IEventService events)
    {
        _node = node;
        _events = events;
        
        _node.OnLog += (message) => _events.InvokeAsync(new LavalinkLogEventArgs(message));
        _node.OnTrackEnded += OnTrackEnded;
        _node.OnTrackException += OnTrackException;
        _node.OnTrackStarted += OnTrackStarted;
        _node.OnTrackStuck += OnTrackStuck;
        _node.OnPlayerUpdated += OnPlayerUpdated;
    }

    public async Task ConnectAsync()
    {
        await _node.ConnectAsync();
    }

    public async Task DisconnectAsync()
    {
        await _node.DisconnectAsync();
    }

    public ILavalinkPlayer GetPlayer(IGuild guild)
    {
        if (_node.TryGetPlayer(guild, out var lavaPlayer))
        {
            return new VictoriaLavalinkPlayer(lavaPlayer, _node);
        }

        return null;
    }

    public bool TryGetPlayer(IGuild guild, out ILavalinkPlayer player)
    {
        if (_node.TryGetPlayer(guild, out var lavaPlayer))
        {
            player = new VictoriaLavalinkPlayer(lavaPlayer, _node);
            return true;
        }

        player = null;
        return false;
    }

    public async Task<ILavalinkPlayer> JoinAsync(IVoiceChannel voiceChannel, ITextChannel textChannel = null)
    {
        var lavaPlayer = await _node.JoinAsync(voiceChannel, textChannel);
        
        return new VictoriaLavalinkPlayer(lavaPlayer, _node);
    }

    public async Task LeaveAsync(IVoiceChannel channel)
    {
        await _node.LeaveAsync(channel);
    }

    public async Task MoveChannelAsync(IVoiceChannel channel)
    {
        await _node.MoveChannelAsync(channel);
    }

    public async Task RebindChannelAsync(ITextChannel channel)
    {
        await _node.MoveChannelAsync(channel);
    }
    
    private Task OnPlayerUpdated(PlayerUpdateEventArgs arg)
    {
        if (arg.Player is not XLavaPlayer player)
        {
            return Task.CompletedTask;;
        }

        return _events.InvokeAsync(new PlayerUpdatedEventArgs(player.Status, arg.Position, player.VoiceChannel?.Guild));
    }

    private Task OnTrackStuck(TrackStuckEventArgs arg)
    {
        if (arg.Player is not XLavaPlayer player)
        {
            return Task.CompletedTask;
        }

        return _events.InvokeAsync(new SoundStuckEventArgs(player.Sound, player.Status, arg.Threshold));
    }

    private Task OnTrackStarted(TrackStartEventArgs arg)
    {
        if (arg.Player is not XLavaPlayer player)
        {
            return Task.CompletedTask;
        }

        return _events.InvokeAsync(new SoundStartedEventArgs(player.Sound, player.Status, player.TextChannel, player.VoiceChannel.Guild));
    }

    private Task OnTrackException(TrackExceptionEventArgs arg)
    {
        if (arg.Player is not XLavaPlayer player)
        {
            return Task.CompletedTask;
        }

        return _events.InvokeAsync(new SoundExceptionEventArgs(player.Status, arg.Exception.Message, player.VoiceChannel.Guild));
    }

    private Task OnTrackEnded(TrackEndedEventArgs arg)
    {
        if (arg.Player is not XLavaPlayer player)
        {
            return Task.CompletedTask;
        }

        return _events.InvokeAsync(new SoundEndedEventArgs(player.VoiceChannel.Guild, new LavaSound(arg.Track), player.Status, (SoundEndedReason)(byte) arg.Reason));
    }
}