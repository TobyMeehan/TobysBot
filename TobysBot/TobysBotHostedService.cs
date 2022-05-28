using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TobysBot.Commands;
using TobysBot.Configuration;

namespace TobysBot;

public class TobysBotHostedService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandHandler _commandHandler;
    private readonly ILogger<TobysBotHostedService> _logger;
    private readonly TobysBotOptions _options;

    public TobysBotHostedService(DiscordSocketClient client, CommandHandler commandHandler, IOptions<TobysBotOptions> options, ILogger<TobysBotHostedService> logger)
    {
        _client = client;
        _commandHandler = commandHandler;
        _logger = logger;
        _options = options.Value;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Ready += ClientReadyAsync;
        _client.Log += LogAsync;

        await _client.LoginAsync(TokenType.Bot, _options.Authorization.Token);
        await _client.StartAsync();
    }

    private Task LogAsync(LogMessage arg)
    {
        _logger.LogInformation("{Source}: {Message}", arg.Source, arg.Message);
        return Task.CompletedTask;
    }

    private async Task ClientReadyAsync()
    {
            await _client.SetActivityAsync(new Game(_options.StartupStatus));

            await _commandHandler.InstallCommandsAsync();

            await _client.SetActivityAsync(new Game(_options.Prefix, ActivityType.Listening));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
    }
}