using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TobysBot.Configuration;
using TobysBot.Data;
using TobysBot.Events;
using TobysBot.Extensions;

namespace TobysBot.Commands;

public class CommandHandler : IEventHandler<MessageReceivedEventArgs>, IEventHandler<SlashCommandExecutedEventArgs>
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    private readonly EmbedService _embeds;
    private readonly IBaseGuildDataService _guildData;
    private readonly IServiceProvider _services;
    private readonly TobysBotOptions _options;
    private readonly ILogger<CommandHandler> _logger;

    public CommandHandler(DiscordSocketClient client, CommandService commandService, EmbedService embeds, IBaseGuildDataService guildData,
        IServiceProvider services, IOptions<TobysBotOptions> options, ILogger<CommandHandler> logger)
    {
        _client = client;
        _commandService = commandService;
        _embeds = embeds;
        _guildData = guildData;
        _services = services;
        _options = options.Value;
        _logger = logger;
    }

    async Task IEventHandler<MessageReceivedEventArgs>.HandleAsync(MessageReceivedEventArgs args)
    {
        if (args.Message is not SocketUserMessage message)
        {
            return;
        }

        var argPos = 0;

        var guild = message.Channel is IGuildChannel guildChannel
            ? await _guildData.GetByDiscordIdAsync(guildChannel.GuildId)
            : null;

        var prefix = guild is null
            ? _options.Prefix
            : guild.Prefix;

        if (!(message.HasStringPrefix(prefix, ref argPos) ||
              message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
        {
            return;
        }

        var context = new SocketGenericCommandContext(_client, message);

        var result = await _commandService.ExecuteAsync(context, argPos, _services);

        _logger.LogInformation("Text command executed \n" +
                               "in guild {GuildId} ({GuildName}) \n" +
                               "in channel {ChannelId} ({ChannelName}) \n" +
                               "by user {UserId} ({Username})",
            context.Guild.Id, context.Guild.Name, context.Channel.Id, context.Channel.Name, context.User.Id, context.User.Username);
        
        if (!result.IsSuccess)
        {
            if (result.Error is CommandError.UnmetPrecondition)
            {
                await HandlePreconditionAsync(context, result);
            }
            
            _logger.LogError("Text command error result: {Error}", result.ErrorReason);
        }
    }

    async Task IEventHandler<SlashCommandExecutedEventArgs>.HandleAsync(SlashCommandExecutedEventArgs args)
    {
        var commandName = args.Command.CommandName;

        foreach (var group in args.Command.Data.Options.Where(x => x.Type is ApplicationCommandOptionType.SubCommandGroup))
        {
            commandName += $" {group.Name}";
        }

        foreach (var subcommand in args.Command.Data.Options.Where(x => x.Type is ApplicationCommandOptionType.SubCommand))
        {
            commandName += $" {subcommand.Name}";
        }
        
        var context = new SocketGenericCommandContext(_client, args.Command);

        var command = _commandService.Commands.FirstOrDefault(
            c => c.Aliases.Contains(commandName));

        if (command is null)
        {
            _logger.LogError("Slash command not found: {Name}", args.Command.CommandName);

            return;
        }

        var preconditionResult = await command.CheckPreconditionsAsync(context, _services);

        if (!preconditionResult.IsSuccess)
        {
            await HandlePreconditionAsync(context, preconditionResult);
            
            _logger.LogError("Slash command precondition failure: {Error}", preconditionResult.ErrorReason);

            return;
        }

        var parameters = command.Parameters.ToDictionary(
                x => x,
                x => x.IsOptional ? x.DefaultValue : null);
        
        foreach (var parameter in parameters.Keys.ToList())
        {
            var option = args.Command.Data.Options.FirstOrDefault(x => x.Name == parameter.Name);
        
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

    private async Task HandlePreconditionAsync(SocketGenericCommandContext context, IResult result)
    {
        await context.Response.ReplyAsync(embed: _embeds.Builder()
            .WithContext(EmbedContext.Error)
            .WithDescription(result.ErrorReason)
            .Build());
    }
}