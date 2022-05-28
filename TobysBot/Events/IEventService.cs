namespace TobysBot.Events;

public interface IEventService
{
    Task InvokeAsync<TArgs>(TArgs args);
}