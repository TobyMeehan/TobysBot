using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TobysBot.Discord.Audio;
using TobysBot.Discord.Audio.Lavalink;
using Victoria;

namespace TobysBot.Discord.Client.Configuration;

public class LavalinkHostedService : IHostedService, IDiscordReadyEventListener
{
    private readonly LavaNode<XLavaPlayer> _node;
    private readonly ILogger<LavalinkHostedService> _logger;
    private readonly IEnumerable<IAudioEventListener> _audioEventListeners;

    public LavalinkHostedService(LavaNode<XLavaPlayer> node, ILogger<LavalinkHostedService> logger, IEnumerable<IAudioEventListener> audioEventListeners)
    {
        _node = node;
        _logger = logger;
        _audioEventListeners = audioEventListeners;
    }
    
    public async Task OnDiscordReady()
    {
        await _node.ConnectAsync();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _node.OnLog += NodeOnLog;

        _node.OnTrackEnded += async args =>
        {
            ITrack track = new LavalinkTrack(args.Track);
            ITextChannel textChannel = args.Player.TextChannel;

            foreach (var listener in _audioEventListeners)
            {
                await listener.OnTrackEnded(track, textChannel);
            }
        };

        _node.OnTrackException += async args =>
        {
            ITrack track = new LavalinkTrack(args.Track);
            ITextChannel textChannel = args.Player.TextChannel;

            foreach (var listener in _audioEventListeners)
            {
                await listener.OnTrackException(track, textChannel, args.ErrorMessage);
            }
        };

        _node.OnTrackStarted += async args =>
        {
            ITrack track = new LavalinkTrack(args.Track);
            ITextChannel textChannel = args.Player.TextChannel;

            foreach (var listener in _audioEventListeners)
            {
                await listener.OnTrackStarted(track, textChannel);
            }
        };

        _node.OnTrackStuck += async args =>
        {
            ITrack track = new LavalinkTrack(args.Track);
            ITextChannel textChannel = args.Player.TextChannel;

            foreach (var listener in _audioEventListeners)
            {
                await listener.OnTrackStuck(track, textChannel, args.Threshold);
            }
        };

        return Task.CompletedTask;
    }

    private Task NodeOnLog(LogMessage arg)
    {
        switch (arg.Severity)
        {
            case LogSeverity.Info:
                _logger.LogInformation(arg.Exception, "{Source}: {Message}", arg.Source, arg.Message);
                break;
            case LogSeverity.Error:
                _logger.LogError(arg.Exception, "{Source}: {Message}",arg.Source, arg.Message);
                break;
            case LogSeverity.Critical:
                _logger.LogCritical(arg.Exception, "{Source}: {Message}",arg.Source, arg.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning(arg.Exception, "{Source}: {Message}",arg.Source, arg.Message);
                break;
            case LogSeverity.Debug:
                _logger.LogDebug(arg.Exception, "{Source}: {Message}",arg.Source, arg.Message);
                break;
            case LogSeverity.Verbose:
                _logger.LogTrace(arg.Exception, "{Source}: {Message}",arg.Source, arg.Message);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _node.DisconnectAsync();
    }
}