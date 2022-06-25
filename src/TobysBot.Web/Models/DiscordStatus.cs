namespace TobysBot.Web.Models;

public class DiscordStatus
{
    public string? ConnectionState { get; set; }
    public int NumberOfGuilds { get; set; }
    public string? Username { get; set; }
}