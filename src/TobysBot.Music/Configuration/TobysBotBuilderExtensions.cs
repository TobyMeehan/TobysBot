using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyAPI.Web;
using TobysBot.Configuration;
using TobysBot.Events;
using TobysBot.Music.Commands;
using TobysBot.Music.Data;
using TobysBot.Music.Events;
using TobysBot.Music.Lyrics;
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
        builder.AddPlugin<MusicPlugin>(services =>
            {
                if (options.Search is not null)
                {
                    services.AddSingleton<LavaNode>();

                    services.AddTransient<ISearchResolver, VictoriaResolver>();
                }

                services.AddTransient<IMusicService, MemoryMusicService>();
                services.AddSingleton<IMemoryQueueService, MemoryQueueService>();

                services.AddTransient<ISearchService, SearchService>();
                services.AddTransient<ISearchResolver, YouTubeResolver>();
                services.AddTransient<ISearchResolver, SavedQueueResolver>();

                services.AddTransient<ILyricsService, LyricsService>();
                services.AddTransient<ILyricsResolver, GeniusLyricsResolver>();
                services.AddTransient<ILyricsResolver, OvhLyricsResolver>();

                services.AddTransient<ISavedQueueDataService, SavedQueueDataService>();

                services.AddTransient<YoutubeClient>();

                if (options.Spotify is not null)
                {
                    switch (options.Spotify)
                    {
                        case { ClientId: null }:
                            throw new NullReferenceException("Spotify client ID not specified.");
                        case { ClientSecret: null }:
                            throw new NullReferenceException("Spotify client secret not specified.");
                    }

                    services.AddSingleton(SpotifyClientConfig
                        .CreateDefault()
                        .WithAuthenticator(new ClientCredentialsAuthenticator(options.Spotify.ClientId,
                            options.Spotify.ClientSecret)));
                    services.AddTransient<ISpotifyClient, SpotifyClient>();

                    services.AddTransient<ISearchResolver, SpotifyResolver>();
                }

                services.SubscribeEvent<PlayerUpdatedEventArgs, TrackProgressEventHandler>();
                services.SubscribeEvent<SoundEndedEventArgs, AutoplayEventHandler>();

                services.SubscribeEvent<SoundStartedEventArgs, TrackNotificationEventHandler>();
                services.SubscribeEvent<SoundExceptionEventArgs, TrackExceptionEventHandler>();

                services.SubscribeEvent<VoiceChannelLeaveEventArgs, VoiceDisconnectedEventHandler>();
            })
            .AddModule<MusicModule>()
            .AddModule<SavedQueueModule>();

        return builder;
    }
}