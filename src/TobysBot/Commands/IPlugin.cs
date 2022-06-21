namespace TobysBot.Commands;

public interface IPlugin
{
    IReadOnlyCollection<IModule> Modules { get; }
    ICommandDictionary<ICommand> Commands { get; }
    
    string? Id { get; }
    string? Name { get; }
    string? Description { get; }
}