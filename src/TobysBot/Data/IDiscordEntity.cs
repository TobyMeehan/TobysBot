namespace TobysBot.Data;

/// <summary>
/// Represents an entity with a Discord ID.
/// </summary>
public interface IDiscordEntity : IEntity
{
    ulong DiscordId { get; }
}