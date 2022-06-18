namespace TobysBot.Data;

public abstract class Entity : IEntity
{
    public virtual string Id { get; set; } = Guid.NewGuid().ToString();
    public virtual DateTimeOffset TimeCreated { get; set; }
}