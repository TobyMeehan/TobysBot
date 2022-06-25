namespace TobysBot.Data;

/// <summary>
/// Represents a relation between an entity and a Discord channel.
/// </summary>
public interface IChannelRelation
{
    ulong ChannelId { get; }
}