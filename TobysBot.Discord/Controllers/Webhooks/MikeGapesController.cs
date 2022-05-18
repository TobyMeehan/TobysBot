using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TobysBot.Discord.Client.Webhooks;
using TobysBot.Discord.Configuration;
using TobysBot.Discord.Models;

namespace TobysBot.Discord.Controllers.Webhooks;

[ApiController]
public class MikeGapesController : ControllerBase
{
    private readonly IWebhookMessageService _webhook;
    private readonly DiscordSocketClient _client;
    private readonly MikeGapesOptions _options;

    public MikeGapesController(IWebhookMessageService webhook, DiscordSocketClient client, IOptions<MikeGapesOptions> options)
    {
        _webhook = webhook;
        _client = client;
        _options = options.Value;
    }
    
    [Route("/webhooks/mikegapes")]
    public async Task<IActionResult> Post(MikeGapesTweetModel tweet)
    {
        var sedecordle = new Regex(@"Daily #\d*\r?\n(.*\r?\n)*#sedecordle");
        var octordle = new Regex(@"Daily Octordle #\d*");
        var quordle = new Regex(@"Daily Quordle \d*");
        var wordle = new Regex(@"Wordle \d* \d/6");
        var thirtytoodle = new Regex(@"I solved 32 wordle games at once at.*#thirtytoodle #wordle");

        Func<MikeGapesServerOptions, ulong> channelSelector = tweet.Text switch
        {
            { } s when sedecordle.IsMatch(s) => x => x.SedecordleChannel,
            { } o when octordle.IsMatch(o) => x => x.OctordleChannel,
            { } q when quordle.IsMatch(q) => x => x.QuordleChannel,
            { } w when wordle.IsMatch(w) => x => x.WordleChannel,
            { } t when thirtytoodle.IsMatch(t) => x => x.ThirtyToodleChannel,
            _ => null
        };

        if (channelSelector is null)
        {
            return Ok();
        }
        
        foreach (var server in _options.Servers)
        {
            var channel = _client
                    .Guilds.Single(x => x.Id == server.Id)
                    .Channels.Single(x => x.Id == channelSelector.Invoke(server))
                as ITextChannel;
            
            await _webhook.SendMessageAsync(channel, tweet.Text, username: "Mike Gapes",
                avatarUrl: "https://pbs.twimg.com/profile_images/1507128393183248392/uKkOLW7v_400x400.jpg");
        }
        
        return Ok();
    }
}
