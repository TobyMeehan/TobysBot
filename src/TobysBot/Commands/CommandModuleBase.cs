using Discord.Commands;
using TobysBot.Commands.Response;

namespace TobysBot.Commands;

public abstract class CommandModuleBase : ModuleBase<SocketGenericCommandContext>
{
    protected ISocketResponseService Response => Context.Response;
}