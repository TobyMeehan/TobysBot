namespace TobysBot.Data;

/// <summary>
/// Represents an entity with a relation to a Discord guild.
/// </summary>
public interface IGuildRelation
{
    ulong GuildId { get; }
}