using Discord;
using Discord.Commands;

namespace TobysBot.Commands;

public interface ICommandService
{
    ICommandDictionary<ICommand> Commands { get; }
    IReadOnlyCollection<IModule> GlobalModules { get; }
    IReadOnlyCollection<IPlugin> Plugins { get; }

    Task InstallCommandsAsync();
    
    Task<IResult> ExecuteAsync(ICommandContext context, int argPos);

    IExecutableCommand Parse(ISlashCommandInteraction command);
}