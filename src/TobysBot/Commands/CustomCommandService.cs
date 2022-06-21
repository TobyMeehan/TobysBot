using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TobysBot.Commands.Builders;
using TobysBot.Events;
using TobysBot.Extensions;

namespace TobysBot.Commands;

public class CustomCommandService : ICommandService
{
    private readonly CommandService _commandService;
    private readonly DiscordSocketClient _client;
    private readonly IEnumerable<IPluginRegistration> _registeredPlugins;
    private readonly IEnumerable<IModuleRegistration> _registeredModules;
    private readonly IServiceProvider _services;

    public CustomCommandService(CommandService commandService, DiscordSocketClient client, 
        IEnumerable<IPluginRegistration> registeredPlugins, 
        IEnumerable<IModuleRegistration> registeredModules, IServiceProvider services)
    {
        _commandService = commandService;
        _client = client;
        _registeredPlugins = registeredPlugins;
        _registeredModules = registeredModules;
        _services = services;
    }

    private List<ModuleBuilder> _globalModules = new();
    private List<PluginBuilder> _plugins = new();

    private IEnumerable<CommandBuilder> Commands =>
        _globalModules.SelectMany(x => x.Commands.Values).Concat(_plugins.SelectMany(x => x.Commands));

    IReadOnlyCollection<ICommand> ICommandService.Commands => Commands.ToList();

    public IReadOnlyCollection<IModule> GlobalModules => _globalModules;
    public IReadOnlyCollection<IPlugin> Plugins => _plugins;
    
    public async Task InstallCommandsAsync()
    {
        foreach (var module in _registeredModules)
        {
            await _commandService.AddModuleAsync(module.ModuleType, _services);
        }

        _plugins.AddRange(_registeredPlugins.Select(x => new PluginBuilder()
            .WithId(x.Id)
            .WithName(x.Name)
            .WithDescription(x.Description)));
        
        foreach (var module in _commandService.Modules)
        {
            var builder = new ModuleBuilder()
                .WithName(module.Name);
            
            if (module.Plugin() is null)
            {
                _globalModules.Add(builder);
            }

            var plugin = _plugins.FirstOrDefault(x => x.Id == module.Plugin());

            if (plugin is null)
            {
                throw new Exception($"Unregistered plugin {module.Plugin()} in module {module.Name}");
            }

            plugin.AddModule(builder);
        }

        foreach (var guild in _client.Guilds)
        {
            await guild.BulkOverwriteApplicationCommandAsync(Commands
                .Select(x => x.Build())
                .Cast<ApplicationCommandProperties>()
                .ToArray());
        }
    }
}