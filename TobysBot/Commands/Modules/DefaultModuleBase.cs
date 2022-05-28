using Discord.Commands;
using TobysBot.Commands.Response;

namespace TobysBot.Commands.Modules;

public class DefaultModuleBase : ModuleBase<SocketGenericCommandContext>
{
    public ISocketResponseService Response => Context.Response;
}