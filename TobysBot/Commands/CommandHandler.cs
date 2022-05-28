using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TobysBot.Configuration;

namespace TobysBot.Commands;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly ModuleCollection _modules;
    private readonly IServiceProvider _services;
    private readonly TobysBotOptions _options;
    private readonly ILogger<CommandHandler> _logger;

    public CommandHandler(DiscordSocketClient client, CommandService commands, ModuleCollection modules,
        IServiceProvider services, IOptions<TobysBotOptions> options, ILogger<CommandHandler> logger)
    {
        _client = client;
        _commands = commands;
        _modules = modules;
        _services = services;
        _options = options.Value;
        _logger = logger;
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

        var debugGuild = _client.Guilds.FirstOrDefault(x => x.Id == _options.DebugGuild);

        foreach (var command in _commands.Commands) // add slash commands
        {
            var slashCommand = new SlashCommandBuilder()
                .WithName(command.Aliases[0])
                .WithDescription(command.Summary)
                .Build();

            await _client.CreateGlobalApplicationCommandAsync(slashCommand);

            debugGuild?.CreateApplicationCommandAsync(slashCommand);
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
            _logger.LogError("Text command error result: {Error}", result.ErrorReason);
        }
    }

    public async Task HandleSlashCommandAsync(SocketSlashCommand arg)
    {
        var context = new SocketGenericCommandContext(_client, arg);

        var command = _commands.Commands.FirstOrDefault(
            c => c.Aliases.Any(alias => alias == arg.CommandName));

        if (command is null)
        {
            _logger.LogError("Slash command not found: {Name}", arg.CommandName);

            return;
        }

        var preconditionResult = await command.CheckPreconditionsAsync(context, _services);

        if (!preconditionResult.IsSuccess)
        {
            _logger.LogError("Slash command precondition failure: {Error}", preconditionResult.ErrorReason);

            return;
        }

        var argList = from option in arg.Data.Options select option.Name;
        var paramList = from option in arg.Data.Options select option.Value;

        var result = await command.ExecuteAsync(context, argList, paramList, _services);

        if (!result.IsSuccess)
        {
            _logger.LogError("Slash command error result: {Error}", result.ErrorReason);
        }
    }
}