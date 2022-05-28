using Microsoft.Extensions.DependencyInjection;
using TobysBot.Events;
using TobysBot.Voice.Events;
using TobysBot.Voice.Lavalink;
using Victoria;

namespace TobysBot.Voice.Configuration;

public class VoiceModuleBuilder
{
    public IServiceCollection Services { get; }

    public VoiceModuleBuilder(IServiceCollection services)
    {
        Services = services;

        services.AddLavaNode<SoundPlayer>(config =>
        {
            config.Hostname = "lavalink.tobymeehan.com";
            config.Authorization = "superstrongpassword";
            config.Port = 80;
            config.EnableResume = true;
            config.ResumeKey = "superstrongpassword";
        });

        services.AddTransient<IVoiceService, LavalinkVoiceService>();

        services.AddHostedService<LavalinkHostedService>();
        
        services.SubscribeEvent<LavalinkLogEventArgs, LavalinkLogger>();
    }
}