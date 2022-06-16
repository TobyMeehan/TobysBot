using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Configuration;
using TobysBot.Misc.Commands;

namespace TobysBot.Misc.Configuration;

public static class TobysBotBuilderExtensions
{
    public static TobysBotBuilder AddMiscModule(this TobysBotBuilder builder, IConfiguration configuration)
    {
        var options = configuration.Get<StarOptions>();

        builder.Services.Configure<StarOptions>(configuration);

        return AddModule(builder, options);
    }

    public static TobysBotBuilder AddMiscModule(this TobysBotBuilder builder, Action<StarOptions> configureOptions)
    {
        var options = new StarOptions();
        configureOptions(options);

        builder.Services.Configure(configureOptions);

        return AddModule(builder, options);
    }
    
    private static TobysBotBuilder AddModule(TobysBotBuilder builder, StarOptions options)
    {
        return builder.AddModule(
            services =>
            {

            },
            commands =>
            {
                commands.AddGlobalModule<ClassicModule>();
            });
    }
}