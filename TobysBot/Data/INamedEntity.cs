namespace TobysBot.Data;

public interface INamedEntity : IEntity
{
    string Name { get; }
}