using System;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;
using TobysBot.Discord.Audio;
using TobysBot.Discord.Audio.Lavalink;
using TobysBot.Discord.Audio.MemoryQueue;
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
        Services.AddLavaNode<XLavaPlayer>(configureOptions);
        Services.AddSingleton<IAudioNode, LavalinkAudioNode>();
        Services.AddSingleton<IAudioSource, LavalinkAudioSource>();
        Services.AddSingleton<IQueue, MemoryQueue>();
        Services.AddTransient<ILyricsProvider, GeniusLyricsProvider>();

        Services.AddTransient<IDiscordReadyEventListener, LavalinkHostedService>();
        Services.AddTransient<IAudioEventListener, AudioEventListener>();
        Services.AddHostedService<LavalinkHostedService>();
  
        return this;
    }
  
    public DiscordClientBuilder AddSpotifyClient(Action<SpotifyOptions> configureOptions = null)
    {
        Services.Configure(configureOptions);

        Services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<SpotifyOptions>>().Value;

            return SpotifyClientConfig
                .CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(options.ClientId, options.ClientSecret));
        });

        Services.AddTransient<ISpotifyClient, SpotifyClient>();

        return this;
    }
  
    public DiscordClientBuilder ConfigureStar(IConfiguration config)
    {
        Services.Configure<StarOptions>(config);

        return this;
    }
}