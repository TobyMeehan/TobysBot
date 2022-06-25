namespace TobysBot.Commands;

/// <summary>
/// Represents a command.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Name (primary alias) of the command.
    /// </summary>
    string? Name { get; }
    
    /// <summary>
    /// Summary of the command.
    /// </summary>
    string? Description { get; }
    
    /// <summary>
    /// Usages of the command.
    /// </summary>
    IReadOnlyCollection<CommandUsage> Usages { get; }
    
    /// <summary>
    /// The command's subcommands.
    /// </summary>
    ICommandDictionary<ICommand> SubCommands { get; }
    
    /// <summary>
    /// The command's options.
    /// </summary>
    IReadOnlyCollection<ICommandOption> Options { get; }
}