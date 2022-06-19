using Discord.Commands;
using TobysBot.Commands;

namespace TobysBot.Extensions;

public static class CommandInfoExtensions
{
    public static string SubCommandGroup(this CommandInfo command)
    {
        var segments = command.Aliases[0].Split(" ");

        return segments.Length switch
        {
            < 3 or > 3 => null,
            3 => segments[0]
        };
    }

    public static string SubCommandParent(this CommandInfo command)
    {
        var segments = command.Aliases[0].Split(" ");

        return segments.Length switch
        {
            < 2 or > 3 => null,
            2 => segments[0],
            3 => segments[1]
        };
    }

    public static string Id(this CommandInfo command)
    {
        return command.Attributes.OfType<IdAttribute>().FirstOrDefault()?.Id ?? command.Aliases[0];
    }

    public static string Id(this ModuleInfo module)
    {
        return module.Attributes.OfType<IdAttribute>().FirstOrDefault()?.Id ?? module.Name;
    }

    public static IEnumerable<CommandInfo> GetAllCommands(this ModuleInfo module)
    {
        foreach (var submodule in module.Submodules)
        {
            foreach (var command in submodule.GetAllCommands())
            {
                yield return command;
            }
        }

        foreach (var command in module.Commands)
        {
            yield return command;
        }
    }
}