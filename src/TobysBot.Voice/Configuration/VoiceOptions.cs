namespace TobysBot.Voice.Configuration;

public class VoiceOptions
{
    public LavalinkOptions Lavalink { get; set; } = new();
    public VoiceDataOptions? Data { get; set; }
}

public class LavalinkOptions
{
    public string? Hostname { get; set; }
    public string? Authorization { get; set; }
    public ushort Port { get; set; } = 2333;
    public bool EnableResume { get; set; }
    public string? ResumeKey { get; set; }
    public bool SelfDeaf { get; set; } = false;

}

public class VoiceDataOptions
{
    public string? SavedPresetCollection { get; set; }
}