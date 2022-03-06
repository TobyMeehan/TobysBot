using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TobysBot.Discord.Audio;
using TobysBot.Discord.Client.TextCommands.Extensions;
using TobysBot.Discord.Client.TextCommands.Extensions.Music;

namespace TobysBot.Discord.Client.TextCommands.Modules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IAudioNode _node;
        private readonly IAudioSource _source;
        
        private IEmote LoopEmote => new Emoji("🔁");
        private IEmote PauseEmote => new Emoji("⏸");
        private IEmote PlayEmote => new Emoji("▶");
        private IEmote SkipEmote => new Emoji("⏭");
        private IEmote StopEmote => new Emoji("⏹");
        private IEmote ClearEmote => new Emoji("⏏");
        
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

            return voiceState.VoiceChannel.Id == Context.Guild.CurrentUser.VoiceChannel?.Id;
        }

        private async Task<bool> EnsureUserInVoiceAsync()
        {
            if (!IsUserInVoiceChannel(out IVoiceState voiceState))
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildJoinVoiceEmbed());
                return false;
            }

            await _node.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);

            return true;
        }

        private async Task<bool> EnsureUserInSameVoiceAsync()
        {
            if (!IsUserInSameVoiceChannel(out IVoiceState voiceState))
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildJoinSameVoiceEmbed());
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
            if (!await EnsureUserInSameVoiceAsync())
            {
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

            IPlayable result;
            
            try
            {
                result = await _source.SearchAsync(query);
            }
            catch (Exception ex)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription($"Error running query: `{ex.Message}`")
                    .Build()
                );
                
                return;
            }

            if (result is null)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildTrackNotFoundEmbed(query));
                return;
            }

            ITrack track;
            
            try
            {
                track = await _node.EnqueueAsync(result, Context.Guild);
            }
            catch (Exception ex)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription($"Error loading track: `{ex.Message}`")
                    .Build()
                );
                
                return;
            }

            if (result is IPlaylist playlist)
            {
                if (track.Url != playlist.First().Url)
                {
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildQueuePlaylistEmbed(playlist));
                }
                else
                {
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildPlayPlaylistEmbed(playlist));
                }
                
                return;
            }

            if (track.Url != result.Url)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildQueueTrackEmbed(result as ITrack));
                return;
            }

            await Context.Message.AddReactionAsync(PlayEmote);
        }

        private async Task ResumeAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var status = _node.Status(Context.Guild);

            if (status is PlayingStatus)
            {
                return;
            }
            
            await _node.ResumeAsync(Context.Guild);
            
            await Context.Message.AddReactionAsync(PlayEmote);
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
            
            await Context.Message.AddReactionAsync(PauseEmote);
        }

        [Command("skip")]
        public async Task SkipAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var track = await _node.SkipAsync(Context.Guild);

            await Context.Message.AddReactionAsync(SkipEmote);
        }

        [Command("stop")]
        public async Task StopAsync()
        {
            if (!await EnsureUserInVoiceAsync())
            {
                return;
            }

            await _node.StopAsync(Context.Guild);
            
            await Context.Message.AddReactionAsync(StopEmote);
        }

        [Command("clear")]
        public async Task ClearAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            await _node.ClearAsync(Context.Guild);
            
            await Context.Message.AddReactionAsync(ClearEmote);
        }

        [Command("np")]
        public async Task NowPlayingAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var status = _node.Status(Context.Guild);

            if (status is not ITrackStatus trackStatus)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildNotPlayingEmbed());
                return;
            }

            await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildTrackStatusEmbed(trackStatus));
        }

        [Command("queue")]
        [Alias("q")]
        public async Task QueueAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var status = _node.Status(Context.Guild);
            var queue = await _node.GetQueueAsync(Context.Guild);

            var trackStatus = status as ITrackStatus;
            
            if (trackStatus is null && queue is null)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildNotPlayingEmbed());
                return;
            }

            await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildQueueEmbed(queue, trackStatus));
        }
    }
}