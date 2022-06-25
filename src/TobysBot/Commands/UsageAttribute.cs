namespace TobysBot.Commands;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class UsageAttribute : Attribute
{
    public CommandUsage Value => new() { CommandName = _commandName, Parameters = _parameters, Description = Summary };

    private readonly string _commandName;
    private readonly string[] _parameters;
    
    public string? Summary { get; }

    public UsageAttribute(string commandName, params string[] parameters)
    {
        _commandName = commandName;
        _parameters = parameters;
    }
}