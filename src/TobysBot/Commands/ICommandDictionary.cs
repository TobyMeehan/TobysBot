namespace TobysBot.Commands;

/// <summary>
/// Collection of commands, indexed by name.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICommandDictionary<out T> : IEnumerable<T> where T : ICommand
{
    T this[string alias] { get; }
}