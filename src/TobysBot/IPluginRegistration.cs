namespace TobysBot;

public interface IPluginRegistration
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
}