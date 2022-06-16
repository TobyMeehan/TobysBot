using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Commands;

namespace TobysBot.Extensions;

public static class CommandServiceExtensions
{
    public static async Task<IEnumerable<ModuleInfo>> InstallCommandsAsync(this CommandService commands, IServiceProvider services)
    {
        var collection = services.GetService<CommandCollection>();

        if (collection is null)
        {
            return Enumerable.Empty<ModuleInfo>();
        }
        
        foreach (var type in collection.GetModules()) // add explicit modules
        {
            await commands.AddModuleAsync(type, services);
        }

        foreach (var assembly in collection.GetAssemblies()) // add assembly modules
        {
            await commands.AddModulesAsync(assembly, services);
        }

        return commands.Modules;
    }
}