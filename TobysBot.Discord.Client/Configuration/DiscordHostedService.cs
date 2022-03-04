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
    }

    private Task ClientOnLog(LogMessage arg)
    {
        LogLevel level = arg.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Debug => LogLevel.Debug,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Warning => LogLevel.Warning,
            _ => LogLevel.None
        };
        
        _logger.Log(level, arg.Exception, "{Message}", arg.Message);

        return Task.CompletedTask;
    }

    private async Task ClientReadyAsync()
    {
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