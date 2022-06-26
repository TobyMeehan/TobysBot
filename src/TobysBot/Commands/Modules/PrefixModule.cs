using Discord;
using Discord.Commands;
using TobysBot.Data;
using TobysBot.Extensions;

namespace TobysBot.Commands.Modules;

public class PrefixModule : CommandModuleBase
{
    private readonly IPrefixDataService _prefixData;
    private readonly EmbedService _embeds;

    public PrefixModule(IPrefixDataService prefixData, EmbedService embeds)
    {
        _prefixData = prefixData;
        _embeds = embeds;
    }
    
    [Command("prefix")]
    [Summary("Sets the prefix for this server.")]
    [RequireContext(ContextType.Guild, ErrorMessage = "Custom prefixes can only be set in guilds.")]
    public async Task PrefixAsync(
        [Summary("New server prefix.")]
        [Remainder] string? prefix = null)
    {
        if (Context.Guild is null)
        {
            throw new NullReferenceException("Guild context was null.");
        }
        
        if (prefix is null)
        {
            prefix = await _prefixData.GetPrefixAsync(Context.Guild.Id);

            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Information)
                .WithDescription($"The current server prefix is **{Format.Sanitize(prefix)}**")
                .Build());
            
            return;
        }

        if (Context.User is not IGuildUser {GuildPermissions.Administrator: true})
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription("You must be an admin to set the prefix.")
                .Build());
            
            return;
        }

        if (prefix.Any(char.IsWhiteSpace))
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription("Prefix cannot contain spaces.")
                .Build());
            
            return;
        }
        
        await _prefixData.SetPrefixAsync(Context.Guild.Id, prefix);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithContext(EmbedContext.Action)
            .WithDescription($"The server prefix is now **{Format.Sanitize(prefix)}**")
            .Build());
    }
}