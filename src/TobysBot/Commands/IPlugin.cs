namespace TobysBot.Commands;

/// <summary>
/// Represents a command plugin.
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// The plugin's modules.
    /// </summary>
    IReadOnlyCollection<IModule> Modules { get; }
    
    /// <summary>
    /// All of the plugin's commands.
    /// </summary>
    ICommandDictionary<ICommand> Commands { get; }
    
    /// <summary>
    /// ID of the plugin.
    /// </summary>
    string? Id { get; }
    
    /// <summary>
    /// Name of the plugin.
    /// </summary>
    string? Name { get; }
    
    /// <summary>
    /// Summary of the plugin.
    /// </summary>
    string? Description { get; }
}