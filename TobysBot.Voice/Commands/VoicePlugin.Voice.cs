using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Voice.Extensions;

namespace TobysBot.Voice.Commands;

public partial class VoicePlugin
{
    public class VoiceModule : VoiceCommandModuleBase
    {
        private readonly EmbedService _embeds;

        private IEmote OkEmote => new Emoji("👌");
    
        public VoiceModule(IVoiceService voiceService, EmbedService embedService) : base(voiceService, embedService)
        {
            _embeds = embedService;
        }
    
        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins the voice channel.")]
        [CheckVoice]
        public async Task JoinAsync()
        {
            await JoinVoiceChannelAsync();

            await Response.ReactAsync(OkEmote);
        }

        [Command("leave", RunMode = RunMode.Async)]
        [Alias("disconnect", "fuckoff")]
        [Summary("Leaves the voice channel.")]
        [CheckVoice(sameChannel: SameChannel.Required)]
        public async Task LeaveAsync()
        {
            await LeaveVoiceChannelAsync();

            await Response.ReactAsync(OkEmote);
        }
    }
}