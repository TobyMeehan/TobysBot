using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyAPI.Web;
using TobysBot.Configuration;
using TobysBot.Events;
using TobysBot.Music.Commands;
using TobysBot.Music.Events;
using TobysBot.Music.MemoryQueue;
using TobysBot.Music.Search;
using TobysBot.Voice.Events;
using Victoria;
using YoutubeExplode;

namespace TobysBot.Music.Configuration;

public static class TobysBotBuilderExtensions
{
    public static TobysBotBuilder AddMusicModule(this TobysBotBuilder builder, IConfiguration configuration)
    {
        var options = configuration.Get<MusicOptions>();

        builder.Services.Configure<MusicOptions>(configuration);

        return AddModule(builder, options);
    }

    public static TobysBotBuilder AddMusicModule(this TobysBotBuilder builder, Action<MusicOptions> configureOptions)
    {
        var options = new MusicOptions();
        configureOptions(options);

        builder.Services.Configure(configureOptions);

        return AddModule(builder, options);
    }
    
    private static TobysBotBuilder AddModule(TobysBotBuilder builder, MusicOptions options)
    {
        return builder.AddModule(
            services =>
            {
                services.AddLavaNode(config =>
                {
                    config.Hostname = options.Search.Hostname;
                    config.Authorization = options.Search.Authorization;
                    config.Port = options.Search.Port;
                });

                services.AddTransient<IMusicService, MemoryMusicService>();
                services.AddSingleton<IMemoryQueueService, MemoryQueueService>();
                
                services.AddTransient<ISearchService, SearchService>();
                services.AddTransient<ISearchResolver, YouTubeResolver>();
                services.AddTransient<ISearchResolver, SpotifyResolver>();
                services.AddTransient<ISearchResolver, VictoriaResolver>();
                
                services.AddTransient<YoutubeClient>();
                
                services.AddSingleton(SpotifyClientConfig
                    .CreateDefault()
                    .WithAuthenticator(new ClientCredentialsAuthenticator(options.Spotify.ClientId,
                        options.Spotify.ClientSecret)));
                services.AddTransient<ISpotifyClient, SpotifyClient>();
                
                services.SubscribeEvent<PlayerUpdatedEventArgs, TrackProgressEventHandler>();
                services.SubscribeEvent<SoundEndedEventArgs, AutoplayEventHandler>();
                
                services.SubscribeEvent<SoundStartedEventArgs, TrackNotificationEventHandler>();
                services.SubscribeEvent<SoundExceptionEventArgs, TrackExceptionEventHandler>();
                
                services.SubscribeEvent<VoiceChannelLeaveEventArgs, VoiceDisconnectedEventHandler>();
            },
            commands =>
            {
                commands.AddPlugin<MusicPlugin>();
            });
    }
}