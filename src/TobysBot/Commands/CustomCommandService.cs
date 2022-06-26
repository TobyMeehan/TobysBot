using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TobysBot.Commands.Builders;
using TobysBot.Configuration;
using TobysBot.Extensions;

namespace TobysBot.Commands;

public class CustomCommandService : ICommandService
{
    private readonly CommandService _commandService;
    private readonly DiscordSocketClient _client;
    private readonly IEnumerable<IPluginRegistration> _registeredPlugins;
    private readonly IEnumerable<IModuleRegistration> _registeredModules;
    private readonly IServiceProvider _services;
    private readonly TobysBotOptions _options;

    public CustomCommandService(CommandService commandService, DiscordSocketClient client, 
        IEnumerable<IPluginRegistration> registeredPlugins, 
        IEnumerable<IModuleRegistration> registeredModules, IServiceProvider services, IOptions<TobysBotOptions> options)
    {
        _commandService = commandService;
        _client = client;
        _registeredPlugins = registeredPlugins;
        _registeredModules = registeredModules;
        _services = services;
        _options = options.Value;
    }

    private readonly List<ModuleBuilder> _globalModules = new();
    private readonly List<PluginBuilder> _plugins = new();
    private readonly CommandDictionary _commands = new();

    ICommandDictionary<ICommand> ICommandService.Commands => _commands;

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
            var builder = ModuleBuilder.Parse(module);
            _commands.AddRange(builder.Commands);
            
            if (module.Plugin() is null)
            {
                _globalModules.Add(builder);
                continue;
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
            if (!_options.SlashCommands)
            {
                await guild.BulkOverwriteApplicationCommandAsync(Array.Empty<ApplicationCommandProperties>());
                continue;
            }
            
            await guild.BulkOverwriteApplicationCommandAsync(_commands
                .Select(x => x.Build())
                .Cast<ApplicationCommandProperties>()
                .ToArray());
        }
    }

    public async Task<IResult> ExecuteAsync(ICommandContext context, int argPos)
    {
        return await _commandService.ExecuteAsync(context, argPos, _services);
    }

    public IExecutableCommand Parse(ISlashCommandInteraction interaction)
    {
        var command = _commands[interaction.Data.Name];
        
        return Parse(interaction.Data.Options, command);
    }

    private IExecutableCommand Parse(IEnumerable<IApplicationCommandInteractionDataOption> data, CommandBuilder command)
    {
        var options = data.ToList();
        
        var subCommand = options.FirstOrDefault(x =>
            x.Type is ApplicationCommandOptionType.SubCommand or ApplicationCommandOptionType.SubCommandGroup);

        if (subCommand is not null)
        {
            return Parse(subCommand.Options, command.SubCommands[subCommand.Name]);
        }

        var arguments = command.Options.ToDictionary(
            x => x,
            x => x.Required ? null : x.DefaultValue);

        foreach (var argument in arguments.Keys.ToList())
        {
            var option = options.FirstOrDefault(x => x.Name == argument.Name);

            if (option is null)
            {
                continue;
            }

            object value = option.Value;

            if (value is long and <= int.MaxValue && argument.Type == typeof(int))
            {
                value = Convert.ToInt32(value);
            }

            arguments[argument] = value;
        }

        return command
            .Executable(_services)
            .WithArguments(arguments.ToDictionary(x => (object)x.Key.Name!, x => x.Value!));
    }
}