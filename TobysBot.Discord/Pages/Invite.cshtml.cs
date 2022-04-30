using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TobysBot.Discord.Pages;

public class Invite : PageModel
{
    private readonly DiscordSocketClient _client;

    public Invite(DiscordSocketClient client)
    {
        _client = client;
    }
    
    public IActionResult OnGet(int permissions = 8)
    {
        var clientId = _client.CurrentUser.Id;
        
        return Redirect(
            @$"https://discord.com/api/oauth2/authorize?client_id={clientId}&permissions={permissions}&scope=bot%20applications.commands");
    }
}