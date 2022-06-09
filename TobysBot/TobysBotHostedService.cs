using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TobysBot.Commands;
using TobysBot.Configuration;
using TobysBot.Events;

namespace TobysBot;

public class TobysBotHostedService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandHandler _commandHandler;
    private readonly IEventService _events;
    private readonly ILogger<TobysBotHostedService> _logger;
    private readonly TobysBotOptions _options;

    public TobysBotHostedService(DiscordSocketClient client, CommandHandler commandHandler, IEventService events, IOptions<TobysBotOptions> options, ILogger<TobysBotHostedService> logger)
    {
        _client = client;
        _commandHandler = commandHandler;
        _events = events;
        _logger = logger;
        _options = options.Value;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Ready += ClientReadyAsync;
        _client.Ready += () => _events.InvokeAsync(new DiscordClientReadyEventArgs());
        _client.Log += (message) => _events.InvokeAsync(new DiscordClientLogEventArgs(message));

        await _client.LoginAsync(TokenType.Bot, _options.Authorization.Token);
        await _client.StartAsync();
    }

    private async Task ClientReadyAsync()
    {
        
        await _client.SetActivityAsync(new Game(_options.StartupStatus));

        try
        {
            await _commandHandler.InstallCommandsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to install commands: {Message}", ex.Message);
        }

        await _client.SetActivityAsync(new Game(_options.Prefix, ActivityType.Listening));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
         await _client.StopAsync();
    }
}