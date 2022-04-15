using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace TobysBot.Discord.Client.Webhooks;

public interface IWebhookMessageService
{
    Task SendMessageAsync(ITextChannel channel, string text = null, IEnumerable<Embed> embeds = null,
        string username = null, string avatarUrl = null);
}