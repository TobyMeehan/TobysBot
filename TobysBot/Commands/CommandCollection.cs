using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Extensions;

namespace TobysBot.Commands;

public class CommandCollection
{
    private List<Type> _modules = new();
    private List<Assembly> _assemblies = new();
    private List<(Type Type, TypeReader TypeReader)> _typeReaders = new();
    
    public void AddModule<T>() where T : IModuleBase
    {
        _modules.Add(typeof(T));
    }

    public void AddModulesFromAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
    }

    public void AddTypeReader<TType, TReader>() where TReader : TypeReader, new()
    {
        _typeReaders.Add((typeof(TType), new TReader()));
    }
    
    public async Task InstallCommandsAsync(IServiceProvider services)
    {
        var commandService = services.GetRequiredService<CommandService>();
        
        foreach (var type in GetModules()) // add explicit modules
        {
            await commandService.AddModuleAsync(type, services);
        }

        foreach (var assembly in GetAssemblies()) // add assembly modules
        {
            await commandService.AddModulesAsync(assembly, services);
        }

        await InstallSlashCommandsAsync(services);
    }

    public async Task InstallSlashCommandsAsync(IServiceProvider services)
    {
        var commandService = services.GetRequiredService<CommandService>();
        var client = services.GetRequiredService<DiscordSocketClient>();

        var slashCommands = commandService.Commands.Select(command => 
            new SlashCommandBuilder()
                .WithName(command.Aliases[0])
                .WithDescription(command.Summary)
                .AddOptions(command.Parameters)
                .Build()).ToList();

        foreach (var guild in client.Guilds)
        {
            await guild.AddSlashCommandsAsync(slashCommands);
        }
    }

    public IEnumerable<Type> GetModules() => _modules;
    public IEnumerable<Assembly> GetAssemblies() => _assemblies;
    public IEnumerable<(Type Type, TypeReader TypeReader)> GetTypeReaders() => _typeReaders;
}