using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TobysBot.Discord.Pages;

public class Invite : PageModel
{
    public IActionResult OnGet()
    {
        return Redirect(
            @"https://discord.com/api/oauth2/authorize?client_id=681894889851715623&permissions=8&scope=bot%20applications.commands");
    }
}