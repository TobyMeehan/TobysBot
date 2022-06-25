namespace TobysBot.Data;

/// <summary>
/// Represents a basic guild with a prefix.
/// </summary>
public interface IGuildData : IDiscordEntity
{
    string Prefix { get; }
}