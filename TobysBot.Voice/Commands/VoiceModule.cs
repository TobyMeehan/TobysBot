using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Voice.Extensions;

namespace TobysBot.Voice.Commands;

public class VoiceModule : VoiceCommandModuleBase
{
    private readonly EmbedService _embeds;

    public VoiceModule(IVoiceService voiceService, EmbedService embedService) : base(voiceService, embedService)
    {
        _embeds = embedService;
    }
    
    [Command("join", RunMode = RunMode.Async)]
    [Summary("Joins the voice channel.")]
    public async Task JoinAsync()
    {
        await JoinVoiceChannelAsync();

        await Response.ReactAsync(new Emoji("👌"), embed: _embeds.Builder()
            .WithJoinVoiceAction()
            .Build());
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

        await Response.ReactAsync(new Emoji("👌"), embed: _embeds.Builder()
            .WithLeaveVoiceAction()
            .Build());
    }
}