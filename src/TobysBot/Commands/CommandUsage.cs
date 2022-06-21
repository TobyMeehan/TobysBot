namespace TobysBot.Commands;

public struct CommandUsage
{
    public string CommandName { get; set; }
    public string[] Parameters { get; set; }
    public string? Description { get; set; }
}