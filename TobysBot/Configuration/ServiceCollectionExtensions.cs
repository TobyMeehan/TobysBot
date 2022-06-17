using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TobysBot.Configuration;

public static class ServiceCollectionExtensions
{
    public static TobysBotBuilder AddTobysBot(this IServiceCollection services)
    {
        return new TobysBotBuilder(services);
    }

    public static TobysBotBuilder AddTobysBot(this IServiceCollection services,
        Action<TobysBotOptions> configureOptions)
    {
        return new TobysBotBuilder(services, configureOptions);
    }

    public static TobysBotBuilder AddTobysBot(this IServiceCollection services, IConfiguration configuration)
    {
        return new TobysBotBuilder(services, configuration);
    }
}