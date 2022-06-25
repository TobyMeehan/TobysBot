namespace TobysBot.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class PluginAttribute : Attribute
{
    /// <summary>
    /// ID of the plugin.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Initialises a new <see cref="PluginAttribute"/> with the specified ID.
    /// </summary>
    /// <param name="pluginId"></param>
    public PluginAttribute(string pluginId)
    {
        Id = pluginId;
    }
}