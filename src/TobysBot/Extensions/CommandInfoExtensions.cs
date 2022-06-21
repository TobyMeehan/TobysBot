using Discord.Commands;
using TobysBot.Commands;

namespace TobysBot.Extensions;

public static class CommandInfoExtensions
{
    public static IEnumerable<CommandUsage> Usage(this CommandInfo commandInfo)
    {
        var usages = commandInfo.Attributes.OfType<UsageAttribute>().Select(x => x.Value with { Description = x.Summary ?? commandInfo.Summary }).ToList();

        if (!usages.Any())
        {
            usages.Add(new CommandUsage
            {
                CommandName = commandInfo.Aliases[0], 
                Parameters = commandInfo.Parameters.Select(x => x.Name).ToArray(), 
                Description = commandInfo.Summary
            });
        }

        return usages;
    }
}