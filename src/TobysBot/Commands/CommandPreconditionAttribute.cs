using Discord.Commands;

namespace TobysBot.Commands;

/// <summary>
/// Requires the module or class to pass the specified precondition before execution can begin
/// </summary>
public abstract class CommandPreconditionAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context is not SocketGenericCommandContext commandContext)
        {
            return PreconditionResult.FromError("Command context mismatch!");
        }

        return await CheckPermissionsAsync(commandContext, command, services);
    }

    /// <summary>
    /// Checks if the command has the sufficient permission to be executed.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="command"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    protected abstract Task<PreconditionResult> CheckPermissionsAsync(SocketGenericCommandContext context,
        CommandInfo command, IServiceProvider services);
}