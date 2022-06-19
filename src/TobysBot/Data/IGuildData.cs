namespace TobysBot.Data;

public interface IGuildData : IDiscordEntity
{
    string Prefix { get; }
}