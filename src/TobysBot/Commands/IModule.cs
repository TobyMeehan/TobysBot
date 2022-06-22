namespace TobysBot.Commands;

public interface IModule
{
    string? Name { get; }
    ICommandDictionary<ICommand> Commands { get; }
}