using Discord.Commands;
using Discord.WebSocket;
using Fergun.Interactive;
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
        services.AddSingleton<ICommandService, CustomCommandService>();
        services.AddSingleton<CommandHandler>();
        services.AddSingleton<IEventService, EventService>();

        services.AddTransient<EmbedService>();
        services.AddSingleton<InteractiveService>();

        services.AddTransient<IBaseGuildDataService, ConfigurationGuildDataService>();

        services.AddTransient<IHostingService, DefaultHostingService>();
        
        services.SubscribeEvent<DiscordClientLogEventArgs, DiscordClientLogger>();
        
        services.SubscribeEvent<MessageReceivedEventArgs, CommandHandler>();
        services.SubscribeEvent<SlashCommandExecutedEventArgs, CommandHandler>();

        services.AddHostedService<TobysBotHostedService>();

        services.AddCommandModule<HelpModule>();
        services.AddCommandModule<PingModule>();
    }

    public TobysBotBuilder(IServiceCollection services, Action<TobysBotOptions> configureOptions) : this(services)
    {
        services.Configure(configureOptions);
    }

    public TobysBotBuilder(IServiceCollection services, IConfiguration configuration) : this(services)
    {
        Services.Configure<TobysBotOptions>(configuration);
    }

    /// <summary>
    /// Adds the specified <see cref="IPluginRegistration"/> to Toby's Bot.
    /// </summary>
    /// <param name="configureServices"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public PluginBuilder<T> AddPlugin<T>(Action<IServiceCollection> configureServices) where T : class, IPluginRegistration
    {
        configureServices(Services);
        
        return Services.AddPlugin<T>();
    }
    
    /// <summary>
    /// Adds a database implementation to Toby's Bot.
    /// </summary>
    /// <param name="configureServices"></param>
    /// <typeparam name="TDataAccess"></typeparam>
    /// <returns></returns>
    public TobysBotBuilder AddDatabase<TDataAccess>(Action<IServiceCollection> configureServices) where TDataAccess : class, IDataAccess
    {
        Services.AddTransient<IDataAccess, TDataAccess>();
        
        Services.AddTransient<IBaseGuildDataService, BaseGuildDataService>();
        Services.AddTransient<IPrefixDataService, PrefixDataService>();

        Services.AddCommandModule<PrefixModule>();

        configureServices(Services);

        return this;
    }

    /// <summary>
    /// Adds the specified <see cref="IHostingService"/> to Toby's Bot.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public TobysBotBuilder AddHostingService<T>() where T : class, IHostingService
    {
        Services.AddTransient<IHostingService, T>();

        return this;
    }
}