namespace TobysBot.Data;

/// <summary>
/// Represents an entity with a relation to a Discord user.
/// </summary>
public interface IUserRelation
{
    ulong UserId { get; }
}