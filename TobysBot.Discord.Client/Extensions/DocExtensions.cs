using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.Commands;
using TobysBot.Discord.Client.TextCommands;

namespace TobysBot.Discord.Client.Extensions;

public static class CommandExtensions
{
    public static IEnumerable<ModuleInfo> GetDocModules(this CommandService commandService)
    {
        return commandService.Modules.Where(m => m.Attributes.OfType<HelpCategoryAttribute>().Any());
    }

    public static string ToHelpString(this CommandInfo command)
    {
        var sb = new StringBuilder();

        sb.Append($"\\{command.Aliases[0]}");

        foreach (var param in command.Parameters)
        {
            sb.Append(" [");
                    
            sb.Append(param.Name);

            if (param.IsOptional)
            {
                sb.Append('?');
            }

            sb.Append(']');
        }

        return sb.ToString();
    }
}