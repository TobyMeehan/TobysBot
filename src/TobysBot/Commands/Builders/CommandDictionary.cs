using System.Collections;

namespace TobysBot.Commands.Builders;

public class CommandDictionary : ICommandDictionary<CommandBuilder>
{
    public CommandDictionary()
    {
        
    }

    public CommandDictionary(IEnumerable<CommandBuilder> commands)
    {
        _commands = commands.ToList();
    }
    
    private readonly List<CommandBuilder> _commands = new();

    public CommandBuilder this[string alias]
    {
        get
        {
            var command = _commands.FirstOrDefault(x => x.Name == alias);

            if (command is null)
            {
                command = new CommandBuilder().WithName(alias);
                _commands.Add(command);
            }

            return command;
        }
    }

    public void AddRange(IEnumerable<CommandBuilder> commands)
    {
        foreach (var command in commands)
        {
            if (command.Name is null)
            {
                throw new NullReferenceException("Command name was null.");
            }

            var existingCommand = this[command.Name]
                .WithDescription(command.Description)
                .WithUsages(command.Usages);

            foreach (var option in command.Options)
            {
                existingCommand.AddOption(option);
            }

            foreach (var subCommand in command.SubCommands)
            {
                existingCommand.AddSubCommand(subCommand);
            }
        }
    }
    
    public IEnumerator<CommandBuilder> GetEnumerator()
    {
        return _commands.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}