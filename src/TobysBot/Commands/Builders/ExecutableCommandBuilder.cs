using Discord.Commands;

namespace TobysBot.Commands.Builders;

public class ExecutableCommandBuilder : IExecutableCommand
{
    private readonly CommandBuilder _command;
    private readonly IServiceProvider _services;

    public ExecutableCommandBuilder(CommandBuilder command, IServiceProvider services)
    {
        _command = command;
        _services = services;
    }

    public string? Name => _command.Name;
    public string? Description => _command.Description;
    public IReadOnlyCollection<CommandUsage> Usages => _command.Usages;
    public ICommandDictionary<ICommand> SubCommands => _command.SubCommands;
    public IReadOnlyCollection<ICommandOption> Options => _command.Options;

    public ExecutableCommandBuilder WithArguments(IEnumerable<KeyValuePair<object, object>> arguments)
    {
        Arguments = arguments.ToDictionary(x => x.Key, x => x.Value);

        return this;
    }
    public Dictionary<object, object> Arguments { get; set; } = new();

    IReadOnlyDictionary<object, object> IExecutableCommand.Arguments => Arguments;

    public async Task<IResult> CheckPreconditionsAsync(ICommandContext context)
    {
        if (_command.CheckPreconditions is null)
        {
            return PreconditionResult.FromSuccess();
        }

        return await _command.CheckPreconditions.Invoke(context, _services);
    }

    public async Task<IResult> ExecuteAsync(ICommandContext context)
    {
        if (_command.Execute is null)
        {
            return ExecuteResult.FromError(CommandError.UnknownCommand, "Execute function not set for this command.");
        }
        
        return await _command.Execute.Invoke(context, Arguments.Keys, Arguments.Values, _services);
    }
}