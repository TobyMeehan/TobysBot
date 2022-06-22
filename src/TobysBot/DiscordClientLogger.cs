using Microsoft.Extensions.Logging;
using TobysBot.Events;
using TobysBot.Extensions;

namespace TobysBot;

public class DiscordClientLogger : IEventHandler<DiscordClientLogEventArgs>
{
    private readonly ILogger<DiscordClientLogger> _logger;

    public DiscordClientLogger(ILogger<DiscordClientLogger> logger)
    {
        _logger = logger;
    }
    
    public Task HandleAsync(DiscordClientLogEventArgs args)
    {
        _logger.LogDiscordMessage(args.Message);
        
        return Task.CompletedTask;
    }
}