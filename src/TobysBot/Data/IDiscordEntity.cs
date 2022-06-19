namespace TobysBot.Data;

public interface IDiscordEntity : IEntity
{
    ulong DiscordId { get; }
}