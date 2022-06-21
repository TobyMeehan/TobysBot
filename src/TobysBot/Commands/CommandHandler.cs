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
    private readonly ICommandService _commandService;
    private readonly EmbedService _embeds;
    private readonly IBaseGuildDataService _guildData;
    private readonly IServiceProvider _services;
    private readonly TobysBotOptions _options;
    private readonly ILogger<CommandHandler> _logger;

    public CommandHandler(DiscordSocketClient client, ICommandService commandService, EmbedService embeds, IBaseGuildDataService guildData,
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

        int argPos = 0;

        var guild = message.Channel is IGuildChannel guildChannel
            ? await _guildData.GetByDiscordIdAsync(guildChannel.GuildId)
            : null;

        bool hasGuildPrefix = guild is not null && message.HasStringPrefix(guild.Prefix, ref argPos);
        bool hasGlobalPrefix = message.HasStringPrefix(_options.Prefix, ref argPos);
        bool hasMentionPrefix = message.HasMentionPrefix(_client.CurrentUser, ref argPos);
        
        if (!(hasGuildPrefix || hasGlobalPrefix || hasMentionPrefix) ||
            message.Author.IsBot)
        {
            return;
        }

        var context = new SocketGenericCommandContext(_client, message);

        var result = await _commandService.ExecuteAsync(context, argPos);

        _logger.LogInformation("Text command executed \n" +
                               "in guild {GuildId} ({GuildName}) \n" +
                               "in channel {ChannelId} ({ChannelName}) \n" +
                               "by user {UserId} ({Username})",
            context.Guild?.Id, context.Guild?.Name, context.Channel.Id, context.Channel.Name, context.User.Id, context.User.Username);
        
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
        var command = _commandService.Parse(args.Command);

        var context = new SocketGenericCommandContext(_client, args.Command);
        
        var preconditionResult = await command.CheckPreconditionsAsync(context);

        if (!preconditionResult.IsSuccess)
        {
            await HandlePreconditionAsync(context, preconditionResult);
            
            _logger.LogError("Slash command precondition failure: {Error}", preconditionResult.ErrorReason);

            return;
        }
        
        var result = await command.ExecuteAsync(context);

        _logger.LogInformation("Slash command '{Command}' executed \n" +
                               "in guild {GuildId} ({GuildName}) \n" +
                               "in channel {ChannelId} ({ChannelName}) \n" +
                               "by user {UserId} ({Username})",
            command.Name, context.Guild?.Id, context.Guild?.Name, context.Channel.Id, context.Channel.Name, context.User.Id, context.User.Username);
        
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