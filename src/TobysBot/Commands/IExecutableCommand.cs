using Discord.Commands;

namespace TobysBot.Commands;

/// <summary>
/// Represents a command which can be executed.
/// </summary>
public interface IExecutableCommand : ICommand
{
    /// <summary>
    /// The command's arguments.
    /// </summary>
    IReadOnlyDictionary<object, object> Arguments { get; }

    /// <summary>
    /// Checks the command's preconditions against the provided command context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<IResult> CheckPreconditionsAsync(ICommandContext context);
    
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<IResult> ExecuteAsync(ICommandContext context);
}