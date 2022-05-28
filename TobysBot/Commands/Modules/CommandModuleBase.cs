using Discord.Commands;
using TobysBot.Commands.Response;

namespace TobysBot.Commands.Modules;

public abstract class CommandModuleBase : ModuleBase<SocketGenericCommandContext>
{
    public ISocketResponseService Response => Context.Response;
}