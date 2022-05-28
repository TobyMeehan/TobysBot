using Microsoft.Extensions.DependencyInjection;

namespace TobysBot.Events;

public class EventService : IEventService
{
    private readonly IServiceProvider _services;

    public EventService(IServiceProvider services)
    {
        _services = services;
    }
    
    public async Task InvokeAsync<TArgs>(TArgs args)
    {
        var handlers = _services.GetServices<IEventHandler<TArgs>>();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(args);
        }
    }
}