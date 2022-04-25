using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TobysBot.Discord.Client.TextCommands;

namespace TobysBot.Discord.Client.Configuration;

public class DiscordHostedService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandHandler _commandHandler;
    private readonly ILogger<DiscordHostedService> _logger;
    private readonly IEnumerable<IDiscordReadyEventListener> _readyEventListeners;
    private readonly DiscordClientOptions _options;

    public DiscordHostedService(DiscordSocketClient client, CommandHandler commandHandler, ILogger<DiscordHostedService> logger, IEnumerable<IDiscordReadyEventListener> readyEventListeners, IOptions<DiscordClientOptions> options)
    {
        _client = client;
        _commandHandler = commandHandler;
        _logger = logger;
        _readyEventListeners = readyEventListeners;
        _options = options.Value;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Ready += ClientReadyAsync;
        _client.Log += ClientOnLog;

        await _commandHandler.InstallCommandsAsync();
        
        await _client.LoginAsync(TokenType.Bot, _options.Token);
        await _client.StartAsync();

        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            
            await _client.SetActivityAsync(new Game($"{_options.Prefix} in " +
                                                    $"{(_client.Guilds.Count < 2 ? "this guild." : $"{_client.Guilds.Count} guilds.")} ",
                ActivityType.Listening));

            await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
        }
    }

    private Task ClientOnLog(LogMessage arg)
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

    private async Task ClientReadyAsync()
    {
        await _client.SetActivityAsync(new Game(_options.Prefix, ActivityType.Listening));
        
        foreach (var listener in _readyEventListeners)
        {
            await listener.OnDiscordReady();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
    }
}