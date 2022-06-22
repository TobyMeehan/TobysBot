using TobysBot.Commands.Builders;

namespace TobysBot.Extensions;

public static class EnumerableExtensions
{
    public static CommandDictionary ToCommandDictionary(this IEnumerable<CommandBuilder> commands)
    {
        return new CommandDictionary(commands);
    }

    public static IEnumerable<T> ForPage<T>(this IEnumerable<T> collection, int page, int perPage)
    {
        return collection.Skip(perPage * page).Take(perPage);
    }
}