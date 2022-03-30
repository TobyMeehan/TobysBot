using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TobysBot.Discord.Audio;
using TobysBot.Discord.Client.TextCommands.Extensions;

namespace TobysBot.Discord.Client.TextCommands.Modules
{
    [Name("Music")]
    [HelpCategory("music")]
    public class MusicModule : VoiceModuleBase
    {
        private readonly IAudioNode _node;
        private readonly IAudioSource _source;
        private readonly ILyricsProvider _lyrics;
        private readonly IHttpClientFactory _httpClientFactory;

        private IEmote LoopEmote => new Emoji("🔁");
        private IEmote PauseEmote => new Emoji("⏸");
        private IEmote PlayEmote => new Emoji("▶");
        private IEmote StopEmote => new Emoji("⏹");
        private IEmote ClearEmote => new Emoji("⏏");
        private IEmote FastForwardEmote => new Emoji("⏩");
        private IEmote ShuffleEmote => new Emoji("🔀");
        private IEmote SkipEmote => new Emoji("⏭");
        private IEmote BackEmote => new Emoji("⏮");
        
        public MusicModule(IAudioNode node, IAudioSource source, ILyricsProvider lyrics, IHttpClientFactory httpClientFactory) : base(node)
        {
            _node = node;
            _source = source;
            _lyrics = lyrics;
            _httpClientFactory = httpClientFactory;
        }

        // Voice Channel
        
        [Command("join", RunMode = RunMode.Async)]
        [Summary("Join the voice channel.")]
        public async Task JoinAsync()
        {
            await EnsureUserInVoiceAsync();
        }

        [Command("leave", RunMode = RunMode.Async)]
        [Alias("disconnect", "fuckoff")]
        [Summary("Leave the voice channel.")]
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
        [Summary("Add the track to the queue or resume playback.")]
        public async Task PlayAsync([Remainder] string query = null)
        {
            if (!await EnsureUserInVoiceAsync(false))
            {
                return;
            }
            
            if (query is null)
            {
                if (!Context.Message.Attachments.Any())
                {
                    await ResumeAsync();
                    return;
                }

                var attachments = await _source.LoadAttachmentsAsync(Context.Message);

                await EnqueueAsync(attachments, null);
                return;
            }

            using var typing = Context.Channel.EnterTypingState();

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

            await EnqueueAsync(result, query);
        }

        private async Task EnqueueAsync(IPlayable playable, string query)
        {
            await JoinAsync();
            
            if (playable is null)
            {
                if (query is not null)
                {
                    await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildTrackNotFoundEmbed(query));
                }
                
                return;
            }

            ITrack track;
            
            try
            {
                track = await _node.EnqueueAsync(playable, Context.Guild);
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

            if (playable is IPlaylist playlist)
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

            if (track.Url != playable.Url)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildQueueTrackEmbed(playable as ITrack));
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
        [Summary("Pause the player.")]
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
        [Summary("Go to the position in the current track.")]
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

            if (timeSpan > track.CurrentTrack.Duration)
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
        
        [Command("stop")]
        [Summary("Stop playback and return to the start of the queue.")]
        public async Task StopAsync()
        {
            if (!await EnsureUserInVoiceAsync())
            {
                return;
            }

            await _node.StopAsync(Context.Guild);
            
            await Context.Message.AddReactionAsync(StopEmote);
        }
        
        // Queue
        
        [Command("skip")]
        [Summary("Skip to the next track.")]
        public async Task SkipAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            await _node.SkipAsync(Context.Guild);

            await Context.Message.AddReactionAsync(SkipEmote);
        }

        [Command("back")]
        [Alias("previous")]
        [Summary("Skip to the previous track.")]
        public async Task BackAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var queue = await _node.GetQueueAsync(Context.Guild);

            if (!queue.Previous().Any())
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("No previous track to skip to.")
                    .Build());
                
                return;
            }

            await _node.BackAsync(Context.Guild);

            await Context.Message.AddReactionAsync(BackEmote);
        }

        [Command("jump")]
        [Alias("skip to")]
        [Summary("Skip to the specified track.")]
        public async Task SkipToAsync(int track)
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            var queue = await _node.GetQueueAsync(Context.Guild);

            if (track < 1 || track > queue.Count())
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("There is not a track at that position in the queue.")
                    .Build());
                
                return;
            }

            await _node.JumpAsync(Context.Guild, track);

            await Context.Message.AddReactionAsync(SkipEmote);
        }

        [Command("clear")]
        [Summary("Clear the queue.")]
        public async Task ClearAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            await _node.ClearAsync(Context.Guild);
            
            await Context.Message.AddReactionAsync(ClearEmote);
        }
        
        [Command("loop")]
        [HideInHelp]
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
            
            if (queue.CurrentTrack is not null && !queue.Next().Any())
            {
                await _node.SetLoopAsync(Context.Guild, new TrackLoopSetting());
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLoopTrackEmbed());
                return;
            }
            
            await _node.SetLoopAsync(Context.Guild, new QueueLoopSetting());
            await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLoopQueueEmbed());
        }

        [Command("loop track")]
        [Summary("Loop the current track.")]
        public async Task LoopTrackAsync()
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

        [Command("loop queue")]
        [Summary("Loop the queue.")]
        public async Task LoopQueueAsync()
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

        [Command("loop off")]
        [Summary("Disable looping.")]
        public async Task LoopOffAsync()
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

            await _node.SetLoopAsync(Context.Guild, new DisabledLoopSetting());
            await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildLoopDisabledEmbed());
        }

        [Command("shuffle")]
        [Summary("Toggle shuffle mode.")]
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

        [Command("shuffle on")]
        [HideInHelp]
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

        [Command("shuffle off")]
        [HideInHelp]
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

        [Command("shuffle once")]
        [Summary("Shuffle the queue.")]
        public async Task ShuffleOnceAsync()
        {
            if (!await EnsureUserInSameVoiceAsync())
            {
                return;
            }

            await _node.ShuffleAsync(Context.Guild);

            await Context.Message.AddReactionAsync(ShuffleEmote);
        }
        
        // Information

        [Command("np")]
        [Summary("Display the currently playing track.")]
        public async Task NowPlayingAsync()
        {
            var status = _node.Status(Context.Guild);

            if (status is not ITrackStatus trackStatus)
            {
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildNotPlayingEmbed());
                return;
            }
            
            var queue = await _node.GetQueueAsync(Context.Guild);

            await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildTrackStatusEmbed(trackStatus, queue));
        }

        [Command("queue")]
        [Alias("q")]
        [Summary("Display the queue.")]
        public async Task QueueAsync()
        {
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
        [Summary("Find lyrics for the currently playing track.")]
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