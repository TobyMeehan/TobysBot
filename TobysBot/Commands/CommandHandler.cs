using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TobysBot.Configuration;
using TobysBot.Extensions;

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

    private SocketGuild GetDebugGuild() =>_client.Guilds.FirstOrDefault(x => x.Id == _options.DebugGuild);

    private IEnumerable<SlashCommandProperties> GetSlashCommands()
    {
        foreach (var command in _commands.Commands)
        {
            var builder = new SlashCommandBuilder()
                .WithName(command.Aliases[0])
                .WithDescription(command.Summary);

            foreach (var parameter in command.Parameters)
            {
                builder.AddOption(new SlashCommandOptionBuilder()
                    .WithName(parameter.Name)
                    .WithDescription(parameter.Summary)
                    .WithRequired(!parameter.IsOptional)
                    .WithType(ParseType(parameter.Type)));
            }
            
            yield return builder.Build();
        }
    }

    private ApplicationCommandOptionType ParseType(Type parameterType)
    {
        if (parameterType == typeof(bool))
        {
            return ApplicationCommandOptionType.Boolean;
        }
        
        if (parameterType == typeof(string))
        {
            return ApplicationCommandOptionType.String;
        }

        if (parameterType == typeof(int))
        {
            return ApplicationCommandOptionType.Integer;
        }

        if (parameterType == typeof(float) || parameterType == typeof(double) || parameterType == typeof(decimal))
        {
            return ApplicationCommandOptionType.Number;
        }

        if (parameterType.IsAssignableTo<IChannel>())
        {
            return ApplicationCommandOptionType.Channel;
        }
        
        if (parameterType.IsAssignableTo<IUser>())
        {
            return ApplicationCommandOptionType.User;
        }

        if (parameterType.IsAssignableTo<IRole>())
        {
            return ApplicationCommandOptionType.Role;
        }

        if (parameterType.IsAssignableTo<IMentionable>())
        {
            return ApplicationCommandOptionType.Mentionable;
        }

        throw new ArgumentOutOfRangeException(nameof(parameterType),"Could not parse slash command type.");
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

        // remove redundant commands
        
        var slashCommands = GetSlashCommands();
        var existingCommands = await _client.GetGlobalApplicationCommandsAsync();
        
        foreach (var redundantCommand in existingCommands.Where(x => slashCommands.Any(c => x.Name == c.Name.Value)))
        {
            await redundantCommand.DeleteAsync();
        }

        var debugGuild = GetDebugGuild();
        
        foreach (var command in slashCommands) // add / re-add slash commands
        {
            await _client.CreateGlobalApplicationCommandAsync(command);
            
            debugGuild?.CreateApplicationCommandAsync(command);
        }
        
        _client.MessageReceived += HandleTextCommandAsync; // subscribe text commands
        _client.SlashCommandExecuted += HandleSlashCommandAsync; // subscribe slash commands
    }

    public async Task UninstallCommandsAsync() // delete guild commands after session
    {
        var debugGuild = GetDebugGuild();

        if (debugGuild is null)
        {
            return;
        }
        
        foreach (var command in await debugGuild.GetApplicationCommandsAsync())
        {
            await command.DeleteAsync();
        }
    }

    public async Task HandleTextCommandAsync(SocketMessage arg)
    {
        if (arg is not SocketUserMessage message)
        {
            return;
        }

        var argPos = 0;

        var prefix = _options.Prefix; // TODO: get prefix from db

        if (!(message.HasStringPrefix(prefix, ref argPos) ||
              message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
        {
            return;
        }

        var context = new SocketGenericCommandContext(_client, message);

        var result = await _commands.ExecuteAsync(context, argPos, _services);

        _logger.LogInformation("Text command executed \n" +
                               "in guild {GuildId} ({GuildName}) \n" +
                               "in channel {ChannelId} ({ChannelName}) \n" +
                               "by user {UserId} ({Username})",
            context.Guild.Id, context.Guild.Name, context.Channel.Id, context.Channel.Name, context.User.Id, context.User.Username);
        
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

        var parameters = command.Parameters.ToDictionary(
                x => x,
                x => x.IsOptional ? x.DefaultValue : null);
        
        foreach (var parameter in parameters.Keys.ToList())
        {
            var option = arg.Data.Options.FirstOrDefault(x => x.Name == parameter.Name);
        
            if (option is null)
            {
                continue;
            }
        
            var value = option.Value;
        
            if (value is long and <= int.MaxValue && parameter.Type == typeof(int))
            {
                value = Convert.ToInt32(value);
            }
            
            parameters[parameter] = value;
        }
        
        var result = await command.ExecuteAsync(context, parameters.Values, parameters.Keys.Select(x => x.Name), _services);

        _logger.LogInformation("Slash command '{Command}' executed \n" +
                               "in guild {GuildId} ({GuildName}) \n" +
                               "in channel {ChannelId} ({ChannelName}) \n" +
                               "by user {UserId} ({Username})",
            command.Name, context.Guild.Id, context.Guild.Name, context.Channel.Id, context.Channel.Name, context.User.Id, context.User.Username);
        
        if (!result.IsSuccess)
        {
            _logger.LogError("Slash command error result: {Error}", result.ErrorReason);
        }
    }
}