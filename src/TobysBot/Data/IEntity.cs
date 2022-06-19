namespace TobysBot.Data;

public interface IEntity
{
    string Id { get; }
    DateTimeOffset TimeCreated { get; }
}