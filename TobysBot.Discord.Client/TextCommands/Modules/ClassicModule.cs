using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using TobysBot.Discord.Client.Configuration;

namespace TobysBot.Discord.Client.TextCommands.Modules;

[HelpCategory("classic")]
public class ClassicModule : ModuleBase<SocketCommandContext>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DiscordClientOptions _options;

    public ClassicModule(IOptions<DiscordClientOptions> options, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }
    
    [Command("pop")]
    [Summary("Calls the user a pop pop head.")]
    public async Task PopAsync(IUser user = null)
    {
        if (user is null)
        {
            await Context.Message.ReplyAsync("You are a pop pop head.");
            return;
        }

        if (user.Id == Context.Client.CurrentUser.Id)
        {
            await Context.Message.ReplyAsync("I am not a pop pop head, how dare you!");
            return;
        }
        
        if (user.Id == _options.TobyId)
        {
            await ReplyAsync($"{user.Mention} is a star. S T A R  S T A R");
        }
        else
        {
            await ReplyAsync($"{user.Mention} is a pop pop head.");
        }

        await Context.Message.DeleteAsync();
    }
}