using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Configuration;
using TobysBot.Util.Commands;
using TobysBot.Util.Data;

namespace TobysBot.Util.Configuration;

public static class TobysBotBuilderExtensions
{
    public static TobysBotBuilder AddUtilPlugin(this TobysBotBuilder builder, IConfiguration configuration)
    {
        builder.AddPlugin<UtilPlugin>(services =>
        {
            services.AddTransient<IReminderService, ReminderService>();
            services.Configure<UtilOptions>(configuration);
        })
            .AddModule<ReminderModule>();

        return builder;
    }
}