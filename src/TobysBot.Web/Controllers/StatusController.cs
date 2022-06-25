using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using TobysBot.Web.Models;

namespace TobysBot.Web.Controllers;

[ApiController]
[Route("/data/[controller]")]
public class StatusController : ControllerBase
{
    private readonly DiscordSocketClient _client;

    public StatusController(DiscordSocketClient client)
    {
        _client = client;
    }
    
    [HttpGet]
    public DiscordStatus Get()
    {
        return new DiscordStatus
        {
            ConnectionState = _client.ConnectionState.ToString(),
            NumberOfGuilds = _client.Guilds.Count,
            Username = _client.CurrentUser.Username
        };
    }
}