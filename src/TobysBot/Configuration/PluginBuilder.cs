using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Commands;

namespace TobysBot.Configuration;

public class PluginBuilder<T> where T : class, IPluginRegistration
{
    public IServiceCollection Services { get; }

    public PluginBuilder(IServiceCollection services)
    {
        Services = services;

        services.AddSingleton<IPluginRegistration, T>();
    }

    public PluginBuilder<T> AddModule<TModule>() where TModule : class, IModuleBase
    {
        Services.AddCommandModule<TModule>();

        return this;
    }
}