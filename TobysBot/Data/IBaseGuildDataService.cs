namespace TobysBot.Data;

public interface IBaseGuildDataService
{
    Task<IGuildData> GetByDiscordIdAsync(ulong id);
}