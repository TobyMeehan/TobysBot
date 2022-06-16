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
}