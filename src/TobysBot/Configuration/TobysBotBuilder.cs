using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Commands;
using TobysBot.Commands.Modules;
using TobysBot.Data;
using TobysBot.Events;
using TobysBot.Hosting;

namespace TobysBot.Configuration;

public class TobysBotBuilder
{
    public IServiceCollection Services { get; }

    public TobysBotBuilder(IServiceCollection services)
    {
        Services = services;

        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<CommandService>();
        services.AddSingleton<CommandHandler>();
        services.AddSingleton<IEventService, EventService>();

        services.AddTransient<EmbedService>();

        services.AddTransient<IBaseGuildDataService, ConfigurationGuildDataService>();

        services.AddTransient<IHostingService, DefaultHostingService>();
        
        services.SubscribeEvent<DiscordClientLogEventArgs, DiscordClientLogger>();
        
        services.SubscribeEvent<MessageReceivedEventArgs, CommandHandler>();
        services.SubscribeEvent<SlashCommandExecutedEventArgs, CommandHandler>();

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

    public PluginBuilder<T> AddPlugin<T>(Action<IServiceCollection> configureServices) where T : class, IPluginRegistration
    {
        configureServices(Services);
        
        return Services.AddPlugin<T>();
    }
    
    public TobysBotBuilder AddDatabase<TDataAccess>(Action<IServiceCollection> configureServices) where TDataAccess : class, IDataAccess
    {
        Services.AddTransient<IDataAccess, TDataAccess>();
        
        Services.AddTransient<IBaseGuildDataService, BaseGuildDataService>();
        Services.AddTransient<IPrefixDataService, PrefixDataService>();

        Services.AddCommandModule<PrefixModule>();

        configureServices(Services);

        return this;
    }

    public TobysBotBuilder AddHostingService<T>() where T : class, IHostingService
    {
        Services.AddTransient<IHostingService, T>();

        return this;
    }
}