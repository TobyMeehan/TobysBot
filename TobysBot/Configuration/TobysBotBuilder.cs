using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Commands;

namespace TobysBot.Configuration;

public class TobysBotBuilder
{
    public IServiceCollection Services { get; }
    public ModuleCollection Modules { get; } = new();

    public TobysBotBuilder(IServiceCollection services)
    {
        Services = services;
        
        services.AddSingleton(Modules);

        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<CommandService>();
        services.AddSingleton<CommandHandler>();

        services.AddHostedService<TobysBotHostedService>();
    }

    public TobysBotBuilder(IServiceCollection services, Action<TobysBotOptions> configureOptions) : this(services)
    {
        services.Configure(configureOptions);
    }

    public TobysBotBuilder(IServiceCollection services, IConfiguration configuration) : this(services)
    {
        Services.Configure<TobysBotOptions>(configuration);
    }

    public TobysBotBuilder AddModule<T>() where T : IModuleBase
    {
        Modules.AddModule<T>();

        return this;
    }

    public TobysBotBuilder AddModulesFromAssembly(Assembly assembly)
    {
        Modules.AddModulesFromAssembly(assembly);

        return this;
    }
}