using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace TobysBot.Discord.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IDiscordClient _discordClient;

        public IndexModel(ILogger<IndexModel> logger, DiscordSocketClient discordClient)
        {
            _logger = logger;
            _discordClient = discordClient;
        }

        public ConnectionState ConnectionState { get; set; }

        public string Activity { get; set; }

        public void OnGet()
        {
            ConnectionState = _discordClient.ConnectionState;
            Activity = _discordClient.CurrentUser?.Activities.FirstOrDefault()?.ToString();
        }
    }
}
