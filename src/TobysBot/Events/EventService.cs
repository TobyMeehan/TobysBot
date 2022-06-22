using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TobysBot.Events;

public class EventService : IEventService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<EventService> _logger;

    public EventService(IServiceProvider services, ILogger<EventService> logger)
    {
        _services = services;
        _logger = logger;
    }
    
    public async Task InvokeAsync<TArgs>(TArgs args)
    {
        var handlers = _services.GetServices<IEventHandler<TArgs>>();

        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync(args);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unhandled exception for handler {Handler} of event {Event}: {Message}", handler.GetType().Name, typeof(TArgs).Name, ex.Message);
            }
        }
    }
}