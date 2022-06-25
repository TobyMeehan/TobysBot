namespace TobysBot.Data;

/// <summary>
/// Represents a database entity.
/// </summary>
public interface IEntity
{
    string Id { get; }
    DateTimeOffset TimeCreated { get; }
}