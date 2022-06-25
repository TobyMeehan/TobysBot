namespace TobysBot.Data;

public interface IPrefixDataService
{
    Task<string> GetPrefixAsync(ulong guild);
    Task SetPrefixAsync(ulong guild, string prefix);
}