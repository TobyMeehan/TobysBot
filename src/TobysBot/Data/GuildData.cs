using Discord;

namespace TobysBot.Data;

public class GuildData : Entity, IGuildData
{
    public GuildData()
    {
        
    }
    
    public GuildData(ulong discordId, string prefix, DateTimeOffset timeCreated)
    {
        DiscordId = discordId;
        Prefix = prefix;
        base.TimeCreated = timeCreated;
    }

    public GuildData(IGuild guild, string prefix)
    {
        DiscordId = guild.Id;
        Prefix = prefix;
        base.TimeCreated = guild.CreatedAt;
    }
    public ulong DiscordId { get; set; }
    public string Prefix { get; set; } = null!;
}