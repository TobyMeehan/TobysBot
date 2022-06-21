using Microsoft.Extensions.Options;
using TobysBot.Configuration;

namespace TobysBot.Data;

public class BaseGuildDataService : IBaseGuildDataService
{
    private readonly IDataAccess _data;
    private readonly TobysBotDataOptions? _options;

    public BaseGuildDataService(IDataAccess data, IOptions<TobysBotOptions> options)
    {
        _data = data;
        _options = options.Value.Data;
    }
    
    public virtual async Task<IGuildData?> GetByDiscordIdAsync(ulong id)
    {
        if (_options?.GuildCollection is null)
        {
            throw new NullReferenceException("Guild collection name not specified.");
        }
        
        return await _data.GetAsync<GuildData>(_options.GuildCollection, id);
    }
}