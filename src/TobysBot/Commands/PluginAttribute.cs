namespace TobysBot.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class PluginAttribute : Attribute
{
    public string Id { get; }

    public PluginAttribute(string pluginId)
    {
        Id = pluginId;
    }
}