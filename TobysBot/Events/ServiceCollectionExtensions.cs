using Microsoft.Extensions.DependencyInjection;

namespace TobysBot.Events;

public static class ServiceCollectionExtensions
{
    public static void SubscribeEvent<TArgs, THandler>(this IServiceCollection services) where THandler : class, IEventHandler<TArgs>
    {
        services.AddTransient<IEventHandler<TArgs>, THandler>();
    }

    public static void SubscribeEvent<TArgs, THandler>(this IServiceCollection services,
        Func<IServiceProvider, THandler> implementation) where THandler : class, IEventHandler<TArgs>
    {
        services.AddTransient<IEventHandler<TArgs>, THandler>(implementation);
    }
}