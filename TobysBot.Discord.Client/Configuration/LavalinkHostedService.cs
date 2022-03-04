using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Victoria;

namespace TobysBot.Discord.Client.Configuration;

public class LavalinkHostedService : IHostedService, IDiscordReadyEventListener
{
    private readonly LavaNode _node;
    private readonly ILogger<LavalinkHostedService> _logger;

    public LavalinkHostedService(LavaNode node, ILogger<LavalinkHostedService> logger)
    {
        _node = node;
        _logger = logger;
    }
    
    public async Task OnDiscordReady()
    {
        await _node.ConnectAsync();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _node.OnLog += NodeOnLog;

        return Task.CompletedTask;
    }

    private Task NodeOnLog(LogMessage arg)
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

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _node.DisconnectAsync();
    }
}