namespace TobysBot.Commands;

public interface ICommandService
{
    IReadOnlyCollection<ICommand> Commands { get; }
    IReadOnlyCollection<IModule> GlobalModules { get; }
    IReadOnlyCollection<IPlugin> Plugins { get; }

    Task InstallCommandsAsync();
}