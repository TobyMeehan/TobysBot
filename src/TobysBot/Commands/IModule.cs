namespace TobysBot.Commands;

public interface IModule
{
    string? Name { get; }
    IReadOnlyCollection<ICommand> Commands { get; }
}