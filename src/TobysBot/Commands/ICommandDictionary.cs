namespace TobysBot.Commands;

public interface ICommandDictionary<out T> : IEnumerable<T> where T : ICommand
{
    T this[string alias] { get; }
}