namespace TobysBot.Voice.Configuration;

public class VoiceOptions
{
    public LavalinkOptions Lavalink { get; set; }
    public VoiceEmbedOptions Embeds { get; set; }
}

public class LavalinkOptions
{
    public string Hostname { get; set; }
    public string Authorization { get; set; }
    public ushort Port { get; set; }
    public bool EnableResume { get; set; }
    public string ResumeKey { get; set; }
    public bool SelfDeaf { get; set; }
    
}

public class VoiceEmbedOptions
{
    public string JoinVoiceErrorDescription { get; set; }
    public string JoinSameVoiceErrorDescription { get; set; }
    public string JoinVoiceAction { get; set; }
    public string LeaveVoiceAction { get; set; }
}