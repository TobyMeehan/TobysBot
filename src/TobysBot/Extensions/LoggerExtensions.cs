using Discord;
using Microsoft.Extensions.Logging;

namespace TobysBot.Extensions;

public static class LoggerExtensions
{
    public static void LogDiscordMessage<T>(this ILogger<T> logger, LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
                logger.LogCritical("{Source}: {Message}", message.Source, message.Message);
                break;
            case LogSeverity.Error:
                logger.LogError("{Source}: {Message}", message.Source, message.Message);
                break;
            case LogSeverity.Warning:
                logger.LogWarning("{Source}: {Message}", message.Source, message.Message);
                break;
            case LogSeverity.Info:
                logger.LogInformation("{Source}: {Message}", message.Source, message.Message);
                break;
            case LogSeverity.Verbose:
                logger.LogTrace("{Source}: {Message}", message.Source, message.Message);
                break;
            case LogSeverity.Debug:
                logger.LogDebug("{Source}: {Message}", message.Source, message.Message);
                break;
        }
    }
}