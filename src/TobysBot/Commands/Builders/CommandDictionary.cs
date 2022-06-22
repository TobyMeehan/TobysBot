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

            this[command.Name]
                .Join(command);
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