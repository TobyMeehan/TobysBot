namespace TobysBot.Commands;

/// <summary>
/// Represents a command module.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Name of the module.
    /// </summary>
    string? Name { get; }
    
    /// <summary>
    /// List of the module's commands.
    /// </summary>
    ICommandDictionary<ICommand> Commands { get; }
}