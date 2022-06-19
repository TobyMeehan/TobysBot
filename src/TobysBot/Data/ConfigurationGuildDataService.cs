using System.Collections.ObjectModel;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TobysBot.Configuration;

namespace TobysBot.Data;

public class ConfigurationGuildDataService : IBaseGuildDataService
{
    private readonly DiscordSocketClient _client;
    private readonly TobysBotOptions _options;

    public ConfigurationGuildDataService(DiscordSocketClient client, IOptions<TobysBotOptions> options)
    {
        _client = client;
        _options = options.Value;
    }
    
    public Task<IGuildData> GetByDiscordIdAsync(ulong id)
    {
        var guild = _client.Guilds.FirstOrDefault(x => x.Id == id);

        if (guild is null)
        {
            return null;
        }

        return Task.FromResult<IGuildData>(new GuildData(guild, _options.Prefix));
    }
}