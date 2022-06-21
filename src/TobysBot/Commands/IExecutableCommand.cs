using Discord.Commands;

namespace TobysBot.Commands;

public interface IExecutableCommand : ICommand
{
    IReadOnlyDictionary<object, object> Arguments { get; }

    Task<IResult> CheckPreconditionsAsync(ICommandContext context);
    Task<IResult> ExecuteAsync(ICommandContext context);
}