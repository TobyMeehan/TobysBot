namespace TobysBot.Configuration;

public class TobysBotOptions
{
    public string Prefix { get; set; } = "\\";
    public string? StartupStatus { get; set; }
    public ulong DebugGuild { get; set; }
    public TobysBotAuthorizationOptions? Authorization { get; set; }
    public TobysBotEmbedOptions? Embeds { get; set; }
    public TobysBotDataOptions? Data { get; set; }
}

public class TobysBotAuthorizationOptions
{
    public string? Token { get; set; }
}

public class TobysBotEmbedOptions
{
    public EmbedColorOptions? Colors { get; set; }
}

public class EmbedColorOptions
{
    public uint Action { get; set; }
    public uint Information { get; set; }
    public uint Error { get; set; }
}

public class TobysBotDataOptions
{
    public string? GuildCollection { get; set; }
}