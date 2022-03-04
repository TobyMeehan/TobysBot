using System;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Discord.Audio;
using TobysBot.Discord.Audio.Lavalink;
using TobysBot.Discord.Client.TextCommands;
using Victoria;

namespace TobysBot.Discord.Client.Configuration;

public class DiscordClientBuilder
{
    public IServiceCollection Services { get; }

    public DiscordClientBuilder(IServiceCollection services, Action<DiscordClientOptions> configureOptions)
    {
        Services = services;

        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<CommandService>();
        services.AddSingleton<CommandHandler>();

        services.AddHostedService<DiscordHostedService>();

        services.Configure(configureOptions);
    }

    public DiscordClientBuilder AddLavaNode(Action<LavaConfig> configureOptions = null)
    {
        Services.AddLavaNode(configureOptions);
        Services.AddSingleton<IAudioNode, LavalinkAudioNode>();
        Services.AddSingleton<IAudioSource, LavalinkAudioSource>();
        Services.AddTransient<IQueue, LavalinkQueue>();

        Services.AddTransient<IDiscordReadyEventListener, LavalinkHostedService>();
        Services.AddHostedService<LavalinkHostedService>();
  
        return this;
    }
}