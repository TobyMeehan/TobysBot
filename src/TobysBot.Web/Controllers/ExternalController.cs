using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;

namespace TobysBot.Web.Controllers;

public class ExternalController : Controller
{
    private readonly DiscordSocketClient _client;

    public ExternalController(DiscordSocketClient client)
    {
        _client = client;
    }
    
    [Route("/invite")]
    public IActionResult Invite(int permissions = 8)
    {
        ulong clientId = _client.CurrentUser.Id;

        return Redirect(
            @$"https://discord.com/api/oauth2/authorize?client_id={clientId}&permissions={permissions}&scope=bot%20applications.commands");
    }

    [Route("/github")]
    public IActionResult Github()
    {
        return Redirect(@"https://github.com/TobyMeehan/TobysBot");
    }
}