namespace TobysBot.Configuration;

public class TobysBotOptions
{
    /// <summary>
    /// The global prefix for the bot to use.
    /// </summary>
    public string Prefix { get; set; } = "\\";
    
    /// <summary>
    /// The status the bot should use on startup.
    /// </summary>
    public string? StartupStatus { get; set; }

    /// <summary>
    /// Whether the bot should register slash commands on startup.
    /// </summary>
    public bool SlashCommands { get; set; }
    
    /// <summary>
    /// The authorization options for the bot.
    /// </summary>
    public TobysBotAuthorizationOptions? Authorization { get; set; }
    
    /// <summary>
    /// The Discord embed options for the bot.
    /// </summary>
    public TobysBotEmbedOptions? Embeds { get; set; }
    
    /// <summary>
    /// The database options for the bot.
    /// </summary>
    public TobysBotDataOptions? Data { get; set; }
}

public class TobysBotAuthorizationOptions
{
    /// <summary>
    /// The Discord bot token.
    /// </summary>
    public string? Token { get; set; }
}

public class TobysBotEmbedOptions
{
    /// <summary>
    /// The colors to be used in embeds.
    /// </summary>
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
    /// <summary>
    /// The database collection containing guild information.
    /// </summary>
    public string? GuildCollection { get; set; }
}