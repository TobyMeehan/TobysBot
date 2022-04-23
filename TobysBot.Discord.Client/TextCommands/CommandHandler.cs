using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TobysBot.Discord.Client.Configuration;

namespace TobysBot.Discord.Client.TextCommands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandHandler> _logger;
        private readonly DiscordClientOptions _options;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider, IOptions<DiscordClientOptions> options, ILogger<CommandHandler> logger)
        {
            _client = client;
            _commands = commands;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(assembly: Assembly.GetExecutingAssembly(), services: _serviceProvider);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage message)
            {
                return;
            }
            
            int argPos = 0;

            if (!(message.HasStringPrefix(_options.Prefix, ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            var result = await _commands.ExecuteAsync(context, argPos: argPos, services: _serviceProvider);

            if (!result.IsSuccess)
            {
                _logger.LogError("Command execution failed: {Error}: {Reason}", result.Error, result.ErrorReason);
            }
        }
    }
}