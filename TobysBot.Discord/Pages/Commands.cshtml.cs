using System.Collections.Generic;
using Discord.Commands;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TobysBot.Discord.Client.Extensions;

namespace TobysBot.Discord.Pages;

public class Commands : PageModel
{
    private readonly CommandService _commandService;

    public Commands(CommandService commandService)
    {
        _commandService = commandService;
    }
    
    public IEnumerable<ModuleInfo> Modules { get; set; }
    
    public void OnGet()
    {
        Modules = _commandService.GetDocModules();
    }
}