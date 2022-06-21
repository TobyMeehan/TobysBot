namespace TobysBot.Commands;

public interface ICommand
{
    string? Name { get; }
    string? Description { get; }
    IReadOnlyCollection<ICommand> SubCommands { get; }
    IReadOnlyCollection<ICommandOption> Options { get; }
}