namespace TobysBot.Commands;

public class CommandGroupAttribute : Attribute
{
    public string Command { get; }
    public string? Group { get; }

    public CommandGroupAttribute(string command)
    {
        Command = command;
    }

    public CommandGroupAttribute(string command, string group)
    {
        Command = command;
        Group = group;
    }
}