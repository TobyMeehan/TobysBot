using Microsoft.Extensions.DependencyInjection;

namespace TobysBot.Configuration;

public static class ServiceCollectionExtensions
{
    public static TobysBotBuilder AddTobysBot(this IServiceCollection services)
    {
        return new TobysBotBuilder(services);
    }
}