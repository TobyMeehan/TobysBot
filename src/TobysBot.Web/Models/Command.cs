namespace TobysBot.Web.Models;

public class Command
{
    public string? Name { get; set; }
    public string? PluginId { get; set; }
    public string[]? Parameters { get; set; }
    public string? Description { get; set; }
}