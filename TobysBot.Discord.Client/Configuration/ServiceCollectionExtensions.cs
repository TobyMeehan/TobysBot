using System;
using Microsoft.Extensions.DependencyInjection;

namespace TobysBot.Discord.Client.Configuration;

public static class ServiceCollectionExtensions
{
    public static DiscordClientBuilder AddDiscordClient(this IServiceCollection services, Action<DiscordClientOptions> configureOptions = null)
    {
        return new DiscordClientBuilder(services, configureOptions);
    }
}