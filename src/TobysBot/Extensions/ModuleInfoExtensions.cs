using Discord.Commands;
using TobysBot.Commands;

namespace TobysBot.Extensions;

public static class ModuleInfoExtensions
{
    public static string? Plugin(this ModuleInfo module)
    {
        return module.Attributes.OfType<PluginAttribute>().FirstOrDefault()?.Id;
    }
}