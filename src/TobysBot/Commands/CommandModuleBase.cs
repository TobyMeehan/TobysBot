using Discord.Commands;
using TobysBot.Commands.Response;

namespace TobysBot.Commands;

/// <summary>
/// Base class for Toby's Bot command modules.
/// </summary>
public abstract class CommandModuleBase : ModuleBase<SocketGenericCommandContext>
{
    /// <summary>
    /// Response service for the command.
    /// </summary>
    protected ISocketResponseService Response => Context.Response;
}