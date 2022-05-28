using Microsoft.Extensions.Logging;
using TobysBot.Events;
using TobysBot.Extensions;
using TobysBot.Voice.Events;

namespace TobysBot.Voice.Lavalink;

public class LavalinkLogger : IEventHandler<LavalinkLogEventArgs>
{
    private readonly ILogger<LavalinkLogger> _logger;

    public LavalinkLogger(ILogger<LavalinkLogger> logger)
    {
        _logger = logger;
    }
    
    public Task HandleAsync(LavalinkLogEventArgs args)
    {
        _logger.LogDiscordMessage(args.Message);
        
        return Task.CompletedTask;
    }
}