using TobysBot.Commands.Builders;

namespace TobysBot.Extensions;

public static class EnumerableExtensions
{
    public static CommandDictionary ToCommandDictionary(this IEnumerable<CommandBuilder> commands)
    {
        return new CommandDictionary(commands);
    }
}