namespace TobysBot.Data;

/// <summary>
/// Simple guild data service used for prefixes.
/// </summary>
public interface IBaseGuildDataService
{
    /// <summary>
    /// Gets the specified guild's data.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IGuildData?> GetByDiscordIdAsync(ulong id);
}