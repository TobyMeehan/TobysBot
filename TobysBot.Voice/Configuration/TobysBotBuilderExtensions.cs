using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Configuration;
using TobysBot.Events;
using TobysBot.Voice.Commands;
using TobysBot.Voice.Events;
using TobysBot.Voice.Lavalink;
using Victoria;

namespace TobysBot.Voice.Configuration;

public static class TobysBotBuilderExtensions
{
    public static TobysBotBuilder AddVoiceModule(this TobysBotBuilder builder, IConfiguration configuration)
    {
        var options = configuration.Get<VoiceOptions>();

        builder.Services.Configure<VoiceOptions>(configuration);
        
        return AddModule(builder, options);
    }

    public static TobysBotBuilder AddVoiceModule(this TobysBotBuilder builder, Action<VoiceOptions> configureOptions)
    {
        var options = new VoiceOptions();
        configureOptions(options);

        builder.Services.Configure(configureOptions);

        return AddModule(builder, options);
    }

    private static TobysBotBuilder AddModule(TobysBotBuilder builder, VoiceOptions options)
    {
        builder.AddModule(
            services =>
            {
                services.AddLavaNode<SoundPlayer>(config =>
                {
                    config.Hostname = options.Lavalink.Hostname;
                    config.Authorization = options.Lavalink.Authorization;
                    config.Port = options.Lavalink.Port;
                    config.EnableResume = options.Lavalink.EnableResume;
                    config.ResumeKey = options.Lavalink.ResumeKey;
                    config.SelfDeaf = options.Lavalink.SelfDeaf;
                });

                services.AddTransient<IVoiceService, LavalinkVoiceService>();

                services.AddHostedService<LavalinkHostedService>();
                
                services.SubscribeEvent<LavalinkLogEventArgs, LavalinkLogger>();
                services.SubscribeEvent<DiscordClientReadyEventArgs, LavalinkHostedService>();
            },
            commands =>
            {
               commands.AddModule<VoiceModule>(); 
            });

        return builder;
    }
}