namespace TobysBot.Data;

/// <summary>
/// Represents an entity with a name.
/// </summary>
public interface INamedEntity : IEntity
{
    string Name { get; }
}