namespace TobysBot.Configuration;

public class TobysBotOptions
{
    public string Prefix { get; set; }
    public string StartupStatus { get; set; }
    public ulong DebugGuild { get; set; }
    public TobysBotAuthorizationOptions Authorization { get; set; }
}

public class TobysBotAuthorizationOptions
{
    public string Token { get; set; }
}