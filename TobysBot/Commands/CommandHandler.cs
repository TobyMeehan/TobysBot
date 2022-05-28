using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TobysBot.Commands;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly ModuleCollection _modules;
    private readonly IServiceProvider _services;

    public CommandHandler(DiscordSocketClient client, CommandService commands, ModuleCollection modules, IServiceProvider services)
    {
        _client = client;
        _commands = commands;
        _modules = modules;
        _services = services;
    }

    public async Task InstallCommandsAsync()
    {
        foreach (var type in _modules.GetModules()) // add explicit modules
        {
            await _commands.AddModuleAsync(type, _services);
        }

        foreach (var assembly in _modules.GetAssemblies()) // add assembly modules
        {
            await _commands.AddModulesAsync(assembly, _services);
        }

        foreach (var command in _commands.Commands) // add slash commands
        {
            await _client.CreateGlobalApplicationCommandAsync(new SlashCommandBuilder()
                .WithName(command.Aliases[0])
                .WithDescription(command.Summary)
                .Build());
        }
        
        _client.MessageReceived += HandleTextCommandAsync; // subscribe text commands
        _client.SlashCommandExecuted += HandleSlashCommandAsync; // subscribe slash commands

    }

    public async Task HandleTextCommandAsync(SocketMessage arg)
    {
        if (arg is not SocketUserMessage message)
        {
            return;
        }

        var argPos = 0;

        var prefix = "\\"; // TODO: get prefix from db

        if (!(message.HasStringPrefix(prefix, ref argPos) ||
              message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
        {
            return;
        }

        var context = new SocketGenericCommandContext(_client, message);

        var result = await _commands.ExecuteAsync(context, argPos, _services);

        if (!result.IsSuccess)
        {
            // TODO: log error
        }
    }

    public async Task HandleSlashCommandAsync(SocketSlashCommand arg)
    {
        var context = new SocketGenericCommandContext(_client, arg);

        var searchResult = _commands.Search(context, 0);

        if (!searchResult.IsSuccess)
        {
            // TODO: log error
            return;
        }

        var command = searchResult.Commands[0].Command;

        var preconditionResult = await command.CheckPreconditionsAsync(context, _services);

        if (!preconditionResult.IsSuccess)
        {
            // TODO: log error
            return;
        }

        var argList = from option in arg.Data.Options select option.Name;
        var paramList = from option in arg.Data.Options select option.Value;
        
        var result = await command.ExecuteAsync(context, argList, paramList, _services);

        if (!result.IsSuccess)
        {
            // TODO: log error
        }
    }
}