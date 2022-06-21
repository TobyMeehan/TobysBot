using Microsoft.Extensions.Options;
using TobysBot.Configuration;

namespace TobysBot.Data;

public class PrefixDataService : IPrefixDataService
{
    private readonly IDataAccess _data;
    private readonly TobysBotOptions _options;

    public PrefixDataService(IDataAccess data, IOptions<TobysBotOptions> options)
    {
        _data = data;
        _options = options.Value;
    }
    
    public async Task<string> GetPrefixAsync(ulong guildId)
    {
        var guild = await _data.GetAsync<GuildData>(_options.Data.GuildCollection, guildId);

        return guild is null ? _options.Prefix : guild.Prefix;
    }

    public async Task SetPrefixAsync(ulong guildId, string prefix)
    {
        var guild = await _data.GetAsync<GuildData>(_options.Data.GuildCollection, guildId);

        guild ??= new GuildData(guildId, prefix, DateTimeOffset.UtcNow);

        guild.Prefix = prefix;

        await _data.SaveAsync(_options.Data.GuildCollection, guild);
    }
}