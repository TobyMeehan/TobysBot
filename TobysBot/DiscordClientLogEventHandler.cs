using Discord;
using Microsoft.Extensions.Logging;
using TobysBot.Events;

namespace TobysBot;

public class DiscordClientLogEventHandler : IEventHandler<DiscordClientLogEventArgs>
{
    private readonly ILogger<DiscordClientLogEventHandler> _logger;

    public DiscordClientLogEventHandler(ILogger<DiscordClientLogEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task HandleAsync(DiscordClientLogEventArgs args)
    {
        switch (args.Message.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical("{Source}: {Message}", args.Message.Source, args.Message.Message);
                break;
            case LogSeverity.Error:
                _logger.LogError("{Source}: {Message}", args.Message.Source, args.Message.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning("{Source}: {Message}", args.Message.Source, args.Message.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation("{Source}: {Message}", args.Message.Source, args.Message.Message);
                break;
            case LogSeverity.Verbose:
                _logger.LogTrace("{Source}: {Message}", args.Message.Source, args.Message.Message);
                break;
            case LogSeverity.Debug:
                _logger.LogDebug("{Source}: {Message}", args.Message.Source, args.Message.Message);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return Task.CompletedTask;
    }
}