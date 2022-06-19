using Discord.Commands;

namespace TobysBot.Commands;

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

    public abstract Task<PreconditionResult> CheckPermissionsAsync(SocketGenericCommandContext context,
        CommandInfo command, IServiceProvider services);
}