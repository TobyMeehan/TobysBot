using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Commands;

namespace TobysBot.Configuration;

public static class ServiceCollectionExtensions
{
    public static TobysBotBuilder AddTobysBot(this IServiceCollection services)
    {
        return new TobysBotBuilder(services);
    }

    public static TobysBotBuilder AddTobysBot(this IServiceCollection services,
        Action<TobysBotOptions> configureOptions)
    {
        return new TobysBotBuilder(services, configureOptions);
    }

    public static TobysBotBuilder AddTobysBot(this IServiceCollection services, IConfiguration configuration)
    {
        return new TobysBotBuilder(services, configuration);
    }

    public static PluginBuilder<T> AddPlugin<T>(this IServiceCollection services) where T : class, IPluginRegistration
    {
        return new PluginBuilder<T>(services);
    }

    public static IServiceCollection AddCommandModule<T>(this IServiceCollection services) where T : class, IModuleBase
    {
        services.AddSingleton<IModuleRegistration, ModuleRegistration<T>>();

        return services;
    }
}