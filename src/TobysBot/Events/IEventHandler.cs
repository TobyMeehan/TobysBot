namespace TobysBot.Events;

public interface IEventHandler<in TArgs>
{
    Task HandleAsync(TArgs args);
}