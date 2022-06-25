using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace TobysBot.Configuration;

public class PluginBuilder<T> where T : class, IPluginRegistration
{
    public IServiceCollection Services { get; }

    public PluginBuilder(IServiceCollection services)
    {
        Services = services;

        services.AddSingleton<IPluginRegistration, T>();
    }

    /// <summary>
    /// Adds a command module to the plugin.
    /// </summary>
    /// <typeparam name="TModule"></typeparam>
    /// <returns></returns>
    public PluginBuilder<T> AddModule<TModule>() where TModule : class, IModuleBase
    {
        Services.AddCommandModule<TModule>();

        return this;
    }
}