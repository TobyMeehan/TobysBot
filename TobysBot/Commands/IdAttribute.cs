namespace TobysBot.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public class IdAttribute : Attribute
{
    public string Id { get; }

    public IdAttribute(string id)
    {
        Id = id;
    }
}