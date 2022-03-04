using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TobysBot.Discord.Audio;

namespace TobysBot.Discord.Client.TextCommands.Modules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IAudioNode _node;
        private readonly IAudioSource _source;

        public MusicModule(IAudioNode node, IAudioSource source)
        {
            _node = node;
            _source = source;
        }

        // Voice Channel
        
        private bool IsUserInVoiceChannel(out IVoiceState voiceState)
        {
            voiceState = Context.User as IVoiceState;
            return voiceState?.VoiceChannel is not null;
        }

        private bool IsUserInSameVoiceChannel(out IVoiceState voiceState)
        {
            if (!IsUserInVoiceChannel(out voiceState))
            {
                return false;
            }

            return voiceState.VoiceChannel == Context.Guild.CurrentUser.VoiceChannel;
        }

        private async Task<bool> EnsureUserInVoiceAsync()
        {
            if (!IsUserInVoiceChannel(out IVoiceState voiceState))
            {
                await ReplyAsync("JOIN VOICE");
                return false;
            }

            await _node.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);

            return true;
        }

        private async Task<bool> EnsureUserInSameVoiceAsync()
        {
            if (!IsUserInSameVoiceChannel(out IVoiceState voiceState))
            {
                await ReplyAsync("JOIN SAME VOICE");
                return false;
            }

            return true;
        }
        
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinAsync()
        {
            await EnsureUserInVoiceAsync();
        }

        [Command("leave", RunMode = RunMode.Async)]
        [Alias("disconnect", "fuckoff")]
        public async Task LeaveAsync()
        {
            if (!IsUserInSameVoiceChannel(out IVoiceState voiceState))
            {
                await ReplyAsync("JOIN SAME VOICE");
                return;
            }

            await _node.LeaveAsync(Context.Guild);
        }
        
        // Player
        [Command("play", RunMode = RunMode.Async)]
        [Alias("p")]
        public async Task PlayAsync([Remainder] string query = null)
        {
            if (query is null)
            {
                await ResumeAsync();
                return;
            }
            
            if (!await EnsureUserInVoiceAsync())
            {
                return;
            }

            var result = await _source.SearchAsync(query);

            if (result is null)
            {
                await ReplyAsync("NO RESULT FOUND");
                return;
            }

            await _node.EnqueueAsync(result, Context.Guild);

            await ReplyAsync($"PLAYING {result.Title}");
        }

        private async Task ResumeAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var status = _node.Status(Context.Guild);

            if (status is not PausedStatus)
            {
                return;
            }
            
            await _node.ResumeAsync(Context.Guild);
        }

        [Command("pause")]
        public async Task PauseAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var status = _node.Status(Context.Guild);

            if (status is not PlayingStatus)
            {
                return;
            }

            await _node.PauseAsync(Context.Guild);
        }
    }
}