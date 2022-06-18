using Discord;

namespace TobysBot.Data;

public class GuildData : IGuildData
{
    public GuildData(ulong discordId, string prefix, DateTime timeCreated)
    {
        Id = null;
        DiscordId = discordId;
        Prefix = prefix;
        TimeCreated = timeCreated;
    }

    public GuildData(IGuild guild, string prefix)
    {
        Id = null;
        DiscordId = guild.Id;
        Prefix = prefix;
        TimeCreated = guild.CreatedAt;
    }

    public string Id { get; }
    public ulong DiscordId { get; }
    public string Prefix { get; }
    public DateTimeOffset TimeCreated { get; }
}