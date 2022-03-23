using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace TobysBot.Discord.Client.TextCommands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IConfiguration config, IServiceProvider serviceProvider)
        {
            _client = client;
            _commands = commands;
            _config = config;
            _serviceProvider = serviceProvider;
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

            if (!message.Embeds.Any())
            {
                await message.ReplyAsync(
                    "https://tenor.com/view/epic-embed-fail-gus-fring-breaking-bad-gustavo-embed-fail-gif-21161041");
            }
            
            int argPos = 0;

            if (!(message.HasStringPrefix(_config.GetSection("Discord")["Prefix"], ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(context, argPos: argPos, services: _serviceProvider);
        }
    }
}