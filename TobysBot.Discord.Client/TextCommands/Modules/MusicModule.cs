using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TobysBot.Discord.Audio;
using TobysBot.Discord.Client.TextCommands.Extensions;
using TobysBot.Discord.Client.TextCommands.Extensions.Music;

namespace TobysBot.Discord.Client.TextCommands.Modules
{
    public class MusicModule : VoiceModuleBase
    {
        private readonly IAudioNode _node;
        private readonly IAudioSource _source;
        private readonly ILyricsProvider _lyrics;

        private IEmote LoopEmote => new Emoji("🔁");
        private IEmote PauseEmote => new Emoji("⏸");
        private IEmote PlayEmote => new Emoji("▶");
        private IEmote StopEmote => new Emoji("⏹");
        private IEmote ClearEmote => new Emoji("⏏");
        private IEmote FastForwardEmote => new Emoji("⏩");
        
        public MusicModule(IAudioNode node, IAudioSource source, ILyricsProvider lyrics) : base(node)
        {
            _node = node;
            _source = source;
            _lyrics = lyrics;
        }

        // Voice Channel
        
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
            using var typing = Context.Channel.EnterTypingState();
            
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

        [Command("seek")]
        public async Task SeekAsync(string position)
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var status = _node.Status(Context.Guild);

            if (status is not ITrackStatus track)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildNotPlayingEmbed());
                return;
            }

            string[] formats = {@"hh\:mm\:ss", @"mm\:ss"};
            
            if (!TimeSpan.TryParseExact(position, formats, null, TimeSpanStyles.None, out var timeSpan))
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("Could not parse a time from that.")
                    .Build());
                
                return;
            }

            if (timeSpan > track.Duration)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("The track is not that long.")
                    .Build());
                
                return;
            }
            
            try
            {
                await _node.SeekAsync(Context.Guild, timeSpan);
                await Context.Message.AddReactionAsync(FastForwardEmote);
            }
            catch (Exception ex)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription($"Failed to seek: `{ex.Message}`")
                    .Build());
            }
        }

        [Group("skip")]
        public class SkipModule : VoiceModuleBase
        {
            
            private IEmote SkipEmote => new Emoji("⏭");
            
            private readonly IAudioNode _node;

            public SkipModule(IAudioNode node) : base(node)
            {
                _node = node;
            }

            [Command]
            public async Task SkipAsync()
            {
                if (!await EnsureUserInSameVoiceAsync())
                {
                    return;
                }

                var track = await _node.SkipAsync(Context.Guild);

                await Context.Message.AddReactionAsync(SkipEmote);
            }

            [Command("to")]
            public async Task ToAsync(int index)
            {
                if (!await EnsureUserInSameVoiceAsync())
                {
                    return;
                }

                var queue = await _node.GetQueueAsync(Context.Guild);

                if (index < 1 || index > queue.Count())
                {
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                        .WithContext(EmbedContext.Error)
                        .WithDescription("There is not a track at that position in the queue.")
                        .Build());
                    
                    return;
                }

                var track = await _node.SkipAsync(Context.Guild, index);

                await Context.Message.AddReactionAsync(SkipEmote);
            }
        }
        
        [Group("shuffle")]
        public class Shuffle : VoiceModuleBase
        {
            private readonly IAudioNode _node;

            private IEmote ShuffleEmote => new Emoji("🔀");
            
            public Shuffle(IAudioNode node) : base(node)
            {
                _node = node;
            }

            [Command]
            public async Task ShuffleAsync()
            {
                if (!await EnsureUserInSameVoiceAsync())
                {
                    return;
                }

                var queue = await _node.GetQueueAsync(Context.Guild);

                switch (queue.ShuffleEnabled)
                {
                    case EnabledShuffleSetting:
                        await ShuffleOffAsync();
                        break;
                        
                    case DisabledShuffleSetting:
                        await ShuffleOnAsync();
                        break;
                }
            }

            [Command("on")]
            public async Task ShuffleOnAsync()
            {
                if (!await EnsureUserInSameVoiceAsync())
                {
                    return;
                }

                await _node.SetShuffleAsync(Context.Guild, new EnabledShuffleSetting());

                await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                    .WithContext(EmbedContext.Action)
                    .WithDescription("Shuffle mode is **enabled**.")
                    .Build());
            }

            [Command("off")]
            public async Task ShuffleOffAsync()
            {
                if (!await EnsureUserInSameVoiceAsync())
                {
                    return;
                }

                await _node.SetShuffleAsync(Context.Guild, new DisabledShuffleSetting());

                await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                    .WithContext(EmbedContext.Action)
                    .WithDescription("Shuffle mode is **disabled**.")
                    .Build());
            }

            [Command("once")]
            public async Task ShuffleOnceAsync()
            {
                if (!await EnsureUserInSameVoiceAsync())
                {
                    return;
                }

                await _node.ShuffleAsync(Context.Guild);

                await Context.Message.AddReactionAsync(ShuffleEmote);
            }
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
        
        [Group("loop")]
        public class LoopModule : VoiceModuleBase
        {
            private readonly IAudioNode _node;

            public LoopModule(IAudioNode node) : base(node)
            {
                _node = node;
            }
            
            [Command]
            public async Task LoopAsync()
            {
                if (!await EnsureUserInSameVoiceAsync())
                {
                    return;
                }

                var queue = await _node.GetQueueAsync(Context.Guild);
                
                if (queue is null)
                {
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildNotPlayingEmbed());
                    return;
                }

                if (queue.LoopEnabled is EnabledLoopSetting)
                {
                    await _node.SetLoopAsync(Context.Guild, new DisabledLoopSetting());
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLoopDisabledEmbed());
                    return;
                }
                
                if (queue.Count() == 1 && queue.CurrentTrack is not null)
                {
                    await _node.SetLoopAsync(Context.Guild, new TrackLoopSetting());
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLoopTrackEmbed());
                    return;
                }
                
                await _node.SetLoopAsync(Context.Guild, new QueueLoopSetting());
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLoopQueueEmbed());
            }

            [Command("track")]
            public async Task TrackAsync()
            {
                if (!await EnsureUserInSameVoiceAsync())
                {
                    return;
                }

                var queue = await _node.GetQueueAsync(Context.Guild);
                
                if (queue is null)
                {
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildNotPlayingEmbed());
                    return;
                }

                if (queue.LoopEnabled is TrackLoopSetting)
                {
                    await _node.SetLoopAsync(Context.Guild, new DisabledLoopSetting());
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLoopDisabledEmbed());
                    return;
                }

                await _node.SetLoopAsync(Context.Guild, new TrackLoopSetting());
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLoopTrackEmbed());
            }

            [Command("queue")]
            public async Task QueueAsync()
            {
                if (!await EnsureUserInSameVoiceAsync())
                {
                    return;
                }

                var queue = await _node.GetQueueAsync(Context.Guild);
                
                if (queue is null)
                {
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildNotPlayingEmbed());
                    return;
                }

                if (queue.LoopEnabled is QueueLoopSetting)
                {
                    await _node.SetLoopAsync(Context.Guild, new DisabledLoopSetting());
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLoopDisabledEmbed());
                    return;
                }

                await _node.SetLoopAsync(Context.Guild, new QueueLoopSetting());
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLoopQueueEmbed());
            }
        }

        [Command("np")]
        public async Task NowPlayingAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var status = _node.Status(Context.Guild);
            var queue = await _node.GetQueueAsync(Context.Guild);

            if (status is not ITrackStatus trackStatus)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildNotPlayingEmbed());
                return;
            }

            await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildTrackStatusEmbed(trackStatus, queue));
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

        [Command("lyrics", RunMode = RunMode.Async)]
        [Alias("ly")]
        public async Task LyricsAsync()
        {
            using var typing = Context.Channel.EnterTypingState();
            
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var status = _node.Status(Context.Guild);

            if (status is not ITrackStatus track)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildNotPlayingEmbed());
                return;
            }

            string lyrics;

            try
            {
                lyrics = await _lyrics.GetLyricsAsync(track.CurrentTrack);
            }
            catch (Exception)
            {
                lyrics = "";
            }

            if (string.IsNullOrWhiteSpace(lyrics))
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription($"No lyrics found for {track.CurrentTrack.Title}.")
                    .Build());
                
                return;
            }
            
            await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLyricsEmbed(track.CurrentTrack, lyrics));
        }
    }
}