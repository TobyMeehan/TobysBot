namespace TobysBot.Voice.Configuration;

public class VoiceEmbedOptions
{
    public string JoinVoiceErrorDescription { get; set; } = "Join the voice channel you square.";
    public string JoinSameVoiceErrorDescription { get; set; } = "We need to be in the same voice channel to do that.";
    public string JoinVoiceAction { get; set; } = "Joining!";
    public string LeaveVoiceAction { get; set; } = "Goodbye!";
}