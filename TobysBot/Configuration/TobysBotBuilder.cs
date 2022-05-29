using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Commands;
using TobysBot.Commands.Modules;
using TobysBot.Events;

namespace TobysBot.Configuration;

public class TobysBotBuilder
{
    public IServiceCollection Services { get; }
    public ModuleCollection Commands { get; } = new();

    public TobysBotBuilder(IServiceCollection services)
    {
        Services = services;
        
        services.AddSingleton(Commands);

        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<CommandService>();
        services.AddSingleton<CommandHandler>();
        services.AddSingleton<IEventService, EventService>();

        services.AddTransient<EmbedService>();
        
        services.SubscribeEvent<DiscordClientLogEventArgs, DiscordClientLogger>();

        services.AddHostedService<TobysBotHostedService>();
        
        Commands.AddModule<PongModule>();
    }

    public TobysBotBuilder(IServiceCollection services, Action<TobysBotOptions> configureOptions) : this(services)
    {
        services.Configure(configureOptions);
    }

    public TobysBotBuilder(IServiceCollection services, IConfiguration configuration) : this(services)
    {
        Services.Configure<TobysBotOptions>(configuration);
    }

    public TobysBotBuilder AddModule(Action<IServiceCollection> configureServices,
        Action<ModuleCollection> configureCommands)
    {
        configureServices(Services);
        configureCommands(Commands);

        return this;
    }
}