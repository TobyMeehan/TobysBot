namespace TobysBot.Data;

public abstract class Entity : IEntity
{
    public virtual string Id { get; set; } = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
        .Replace("/", "-")
        .Replace("+", "_")
        .Replace("=", "");
    public virtual DateTimeOffset TimeCreated { get; set; }
}