using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Commands.Modules;

namespace TobysBot.Voice.Commands;

public class VoiceModule : VoiceCommandModuleBase
{
    public VoiceModule(IVoiceService voiceService, EmbedService embedService) : base(voiceService, embedService)
    {
    }
    
    [Command("join", RunMode = RunMode.Async)]
    [Summary("Joins the voice channel.")]
    public async Task JoinAsync()
    {
        await JoinVoiceChannelAsync();
    }

    [Command("leave", RunMode = RunMode.Async)]
    [Alias("disconnect", "fuckoff")]
    [Summary("Leaves the voice channel.")]
    public async Task LeaveAsync()
    {
        if (!await EnsureUserInSameVoiceAsync())
        {
            return;
        }

        await LeaveVoiceChannelAsync();
    }
}