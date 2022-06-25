using Discord;
using Discord.Commands;

namespace TobysBot.Commands;

/// <summary>
/// Service for interacting with commands.
/// </summary>
public interface ICommandService
{
    /// <summary>
    /// List of all registered commands.
    /// </summary>
    ICommandDictionary<ICommand> Commands { get; }
    
    /// <summary>
    /// List of all globally registered command modules.
    /// </summary>
    IReadOnlyCollection<IModule> GlobalModules { get; }
    
    /// <summary>
    /// List of all command plugins.
    /// </summary>
    IReadOnlyCollection<IPlugin> Plugins { get; }

    /// <summary>
    /// Installs text commands to the command service, and registers slash commands with Discord.
    /// </summary>
    /// <returns></returns>
    Task InstallCommandsAsync();
    
    /// <summary>
    /// Parses the command context and executes the command.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="argPos"></param>
    /// <returns></returns>
    Task<IResult> ExecuteAsync(ICommandContext context, int argPos);

    /// <summary>
    /// Parses the slash command, and returns an executable command representation.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    IExecutableCommand Parse(ISlashCommandInteraction command);
}