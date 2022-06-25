using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Commands;

namespace TobysBot.Configuration;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Toby's Bot to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static TobysBotBuilder AddTobysBot(this IServiceCollection services)
    {
        return new TobysBotBuilder(services);
    }

    /// <summary>
    /// Adds Toby's Bot to the <see cref="IServiceCollection"/> using the specified <see cref="TobysBotOptions"/> action.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static TobysBotBuilder AddTobysBot(this IServiceCollection services,
        Action<TobysBotOptions> configureOptions)
    {
        return new TobysBotBuilder(services, configureOptions);
    }

    /// <summary>
    /// Adds Toby's Bot to the <see cref="IServiceCollection"/> using the specified <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static TobysBotBuilder AddTobysBot(this IServiceCollection services, IConfiguration configuration)
    {
        return new TobysBotBuilder(services, configuration);
    }

    /// <summary>
    /// Adds the specified <see cref="IPluginRegistration"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static PluginBuilder<T> AddPlugin<T>(this IServiceCollection services) where T : class, IPluginRegistration
    {
        return new PluginBuilder<T>(services);
    }

    /// <summary>
    /// Adds the specified module to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddCommandModule<T>(this IServiceCollection services) where T : class, IModuleBase
    {
        services.AddSingleton<IModuleRegistration, ModuleRegistration<T>>();

        return services;
    }
}