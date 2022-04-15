using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;

namespace TobysBot.Discord.Client.Webhooks;

public class WebhookMessageService : IWebhookMessageService
{
    public async Task SendMessageAsync(ITextChannel channel, string text = null, IEnumerable<Embed> embeds = null, string username = null, string avatarUrl = null)
    {
        var name = Guid.NewGuid().ToString();
        var webhook = await channel.CreateWebhookAsync(name);
        
        using var webhookClient = new DiscordWebhookClient(webhook);
        
        await webhookClient.SendMessageAsync(text, embeds: embeds, username: username, avatarUrl: avatarUrl);

        await webhookClient.DeleteWebhookAsync();
    }
}