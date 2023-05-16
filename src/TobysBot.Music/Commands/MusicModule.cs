using System.Globalization;
using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Music.Extensions;
using TobysBot.Music.Lyrics;
using TobysBot.Music.Search.Result;
using TobysBot.Voice;
using TobysBot.Voice.Commands;
using TobysBot.Voice.Status;

namespace TobysBot.Music.Commands;

[Plugin("music")]
public class MusicModule : VoiceCommandModuleBase
{
    private readonly EmbedService _embeds;
    private readonly ISearchService _search;
    private readonly IMusicService _music;
    private readonly ILyricsService _lyrics;

    private static IEmote PauseEmote => new Emoji("⏸");
    private static IEmote PlayEmote => new Emoji("▶");
    private static IEmote StopEmote => new Emoji("⏹");
    private static IEmote ClearEmote => new Emoji("⏏");
    private static IEmote FastForwardEmote => new Emoji("⏩");
    private static IEmote RewindEmote => new Emoji("⏪");
    private static IEmote SkipEmote => new Emoji("⏭");
    private static IEmote BackEmote => new Emoji("⏮");
    private static IEmote MoveEmote => new Emoji("↔");
    private static IEmote RemoveEmote => new Emoji("⤴");

    private const string GuildRequiredErrorMessage = "You must be in a guild to play music.";

    public MusicModule(IVoiceService voiceService, EmbedService embeds, ISearchService search,
        IMusicService music, ILyricsService lyrics) :
        base(voiceService)
    {
        _embeds = embeds;
        _search = search;
        _music = music;
        _lyrics = lyrics;
    }

    [Command("play", RunMode = RunMode.Async)]
    [Alias("p")]
    [Summary("Loads query and adds it to the queue.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.IfBotConnected)]
    public async Task PlayAsync(
        [Summary("Url or search for track to play.")] [Remainder]
        string? query = null)
    {
        using var response = await Response.DeferAsync();

        var search = query switch
        {
            null when Context.Message is not null => await _search.LoadAttachmentsAsync(Context.Message, Context.User),
            not null => await _search.SearchAsync(query, Context.User),
            _ => null
        };

        switch (search)
        {
            case null:
                await response.ModifyResponseAsync(x =>
                {
                    x.Embed = _embeds.Builder()
                        .WithContext(EmbedContext.Error)
                        .WithDescription("Please provide a search query.")
                        .Build();
                });

                return;
            case NotFoundSearchResult:
                await response.ModifyResponseAsync(x =>
                {
                    x.Embed = _embeds.Builder()
                        .WithNotFoundError()
                        .Build();
                });

                return;

            case LoadFailedSearchResult loadFailed:
                await response.ModifyResponseAsync(x =>
                {
                    x.Embed = _embeds.Builder()
                        .WithLoadFailedError(loadFailed)
                        .Build();
                });

                return;
        }

        await JoinVoiceChannelAsync();

        switch (search)
        {
            case TrackResult track:
                var playing = await _music.EnqueueAsync(Context.Guild!, track.Track);

                if (playing == track.Track)
                {
                    await response.ReactAsync(PlayEmote);
                    break;
                }

                await response.ModifyResponseAsync(x =>
                {
                    x.Embed = _embeds.Builder()
                        .WithQueueTrackAction(track.Track)
                        .Build();
                });

                break;

            case PlaylistResult playlist:
                await _music.EnqueueAsync(Context.Guild!, playlist.Playlist.Tracks);

                await response.ModifyResponseAsync(x =>
                {
                    x.Embed = _embeds.Builder()
                        .WithQueuePlaylistAction(playlist.Playlist)
                        .Build();
                });

                break;

            case SavedQueueResult savedQueue:
                await _music.EnqueueAsync(Context.Guild!, savedQueue.Queue);

                await response.ModifyResponseAsync(x =>
                {
                    x.Embed = _embeds.Builder()
                        .WithQueueSavedQueueAction(savedQueue.Queue)
                        .Build();
                });

                break;
        }
    }

    [Command("unpause")]
    [Alias("resume")]
    [Summary("Resumes playback.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task ResumeAsync()
    {
        if (Status is not PlayingStatus)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        if (Status is PlayingStatus { IsPaused: false })
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithAlreadyPlayingError()
                .Build());
            return;
        }

        await _music.ResumeAsync(Context.Guild!);

        await Response.ReactAsync(PlayEmote);
    }

    [Command("pause")]
    [Summary("Pauses playback.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task PauseAsync()
    {
        if (Status is not PlayingStatus)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        if (Status is PlayingStatus { IsPaused: true })
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithAlreadyPausedError()
                .Build());

            return;
        }

        await _music.PauseAsync(Context.Guild!);

        await Response.ReactAsync(PauseEmote);
    }

    [Command("seek")]
    [Summary("Skips to the timestamp in the current track.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task SeekAsync(
        [Summary("Timestamp in the current track to skip to.")]
        TimeSpan timestamp)
    {
        var track = await _music.GetTrackAsync(Context.Guild!);

        if (track is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        if (timestamp > track.Duration)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithTimestampTooLongError()
                .Build());

            return;
        }

        await _music.SeekAsync(Context.Guild!, timestamp);

        await Response.ReactAsync(FastForwardEmote);
    }

    [Command("fastforward")]
    [Alias("ff")]
    [Summary("Fast forwards the track by the specified amount.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task FastForwardAsync(
        [Summary("Number of seconds to fast forward by.")]
        int seconds = 10)
    {
        var track = await _music.GetTrackAsync(Context.Guild!);

        if (track is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        if (seconds < 0)
        {
            await RewindAsync(-seconds);
            return;
        }

        var timeSpan = track.Position + TimeSpan.FromSeconds(seconds);

        if (timeSpan > track.Duration)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithFastForwardTooFarError()
                .Build());

            return;
        }

        await _music.SeekAsync(Context.Guild!, timeSpan);

        await Response.ReactAsync(FastForwardEmote);
    }

    [Command("rewind")]
    [Alias("rw")]
    [Summary("Rewinds the track by the specified amount.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task RewindAsync(
        [Summary("Number of seconds to rewind by.")]
        int seconds = 10)
    {
        var track = await _music.GetTrackAsync(Context.Guild!);

        if (track is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        if (seconds < 0)
        {
            await FastForwardAsync(-seconds);
            return;
        }

        var timeSpan = track.Position - TimeSpan.FromSeconds(seconds);

        if (timeSpan < TimeSpan.Zero)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithRewindTooFarError()
                .Build());

            return;
        }

        await _music.SeekAsync(Context.Guild!, timeSpan);

        await Response.ReactAsync(RewindEmote);
    }

    [Command("stop")]
    [Summary("Stops playback and returns to the start of the queue.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task StopAsync()
    {
        await _music.StopAsync(Context.Guild!);

        await Response.ReactAsync(StopEmote);
    }

    [Command("skip")]
    [Summary("Skips to the next track.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task SkipAsync()
    {
        await _music.SkipAsync(Context.Guild!);

        await Response.ReactAsync(SkipEmote);
    }

    [Command("back")]
    [Alias("previous")]
    [Summary("Skips to the previous track.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task BackAsync()
    {
        var queue = await _music.GetQueueAsync(Context.Guild!);

        if (!queue.Previous.Any())
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNoPreviousTrackError()
                .Build());

            return;
        }

        await _music.BackAsync(Context.Guild!);

        await Response.ReactAsync(BackEmote);
    }

    [Command("jump")]
    [Summary("Jumps to the specified track.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task JumpAsync(
        [Summary("Position to jump to.")] int track)
    {
        var queue = await _music.GetQueueAsync(Context.Guild!);

        if (track < 1 || track > queue.Length)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithPositionOutOfRangeError()
                .Build());

            return;
        }

        await _music.JumpAsync(Context.Guild!, track);

        await Response.ReactAsync(SkipEmote);
    }

    [Command("clear")]
    [Summary("Clears the queue.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task ClearAsync()
    {
        await _music.ClearAsync(Context.Guild!);

        await Response.ReactAsync(ClearEmote);
    }

    [Command("loop track")]
    [Summary("Toggles looping the current track.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public Task LoopTrackAsync() => LoopAsync(new TrackLoopSetting());

    [Command("loop queue")]
    [Summary("Toggles looping the queue.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public Task LoopQueueAsync() => LoopAsync(new QueueLoopSetting());

    [Command("loop off")]
    [Summary("Disables looping.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public Task LoopOffAsync() => LoopAsync(new DisabledLoopSetting());

    [Command("loop toggle")]
    [Alias("loop")]
    [Summary("Toggles looping.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public Task LoopToggleAsync() => LoopAsync();

    private async Task LoopAsync(ILoopSetting? setting = null)
    {
        var queue = await _music.GetQueueAsync(Context.Guild!);

        if (queue is null or {Empty: true})
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        switch (setting)
        {
            case DisabledLoopSetting:
            case null when queue.Loop is EnabledLoopSetting:
            case TrackLoopSetting when queue.Loop is TrackLoopSetting:
            case QueueLoopSetting when queue.Loop is QueueLoopSetting:
                setting = new DisabledLoopSetting();
                break;

            case TrackLoopSetting:
            case null when queue.CurrentTrack is not null && !queue.Next.Any():
                setting = new TrackLoopSetting();
                break;

            case QueueLoopSetting:
            case null:
                setting = new QueueLoopSetting();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(setting), setting, "Invalid loop mode.");
        }

        await _music.SetLoopAsync(Context.Guild!, setting);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithLoopAction(setting)
            .Build());
    }

    [Command("shuffle")]
    [Summary("Toggle shuffle mode.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task ShuffleAsync()
    {
        var queue = await _music.GetQueueAsync(Context.Guild!);

        await _music.SetShuffleAsync(Context.Guild!, !queue.Shuffle);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithShuffleAction(!queue.Shuffle)
            .Build());
    }

    [Command("move")]
    [Alias("mv")]
    [Summary("Moves the specified track to the specified position.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task MoveAsync(
        [Summary("Position of track to move.")]
        int track,
        [Summary("Position to move track to.")]
        int position)
    {
        var queue = await _music.GetQueueAsync(Context.Guild!);

        if (queue.Empty)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        if (track > queue.Length || track < 1)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithPositionOutOfRangeError()
                .Build());

            return;
        }

        if (position > queue.Length || position < 1)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithPositionOutOfRangeError()
                .Build());

            return;
        }

        await _music.MoveAsync(Context.Guild!, track, position);

        await Response.ReactAsync(MoveEmote);
    }

    [Command("remove track")]
    [Alias("remove", "rm")]
    [Summary("Removes the specified track from the queue.")]
    [Usage("remove", "track")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task RemoveAsync(
        [Summary("Position of track to remove.")]
        int track)
    {
        var queue = await _music.GetQueueAsync(Context.Guild!);

        if (queue.Empty)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        if (track > queue.Length || track < 1)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithPositionOutOfRangeError()
                .Build());

            return;
        }

        await _music.RemoveAsync(Context.Guild!, track);

        await Response.ReactAsync(RemoveEmote);
    }

    [Command("remove range")]
    [Alias("rmrange", "rm range")]
    [Summary("Removes the specified range of tracks from the queue.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task RemoveRangeAsync(
        [Summary("Position of first track to remove.")]
        int start,
        [Summary("Position of last track to remove.")]
        int end)
    {
        var queue = await _music.GetQueueAsync(Context.Guild!);

        if (queue.Empty)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        if (start > queue.Length || start < 1 || end > queue.Length || end < 1)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithPositionOutOfRangeError()
                .Build());

            return;
        }

        await _music.RemoveRangeAsync(Context.Guild!, start, end);

        await Response.ReactAsync(RemoveEmote);
    }

    [Command("lyrics", RunMode = RunMode.Async)]
    [Alias("ly")]
    [Summary("Finds lyrics for the currently playing track.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task LyricsAsync()
    {
        var track = await _music.GetTrackAsync(Context.Guild!);

        if (track is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        using var response = await Response.DeferAsync();
        
        var result = await _lyrics.GetLyricsAsync(track);

        if (!result.Success)
        {
            await response.ModifyResponseAsync(x => x.Embed = _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription($"No results for [{track.Title}]({track.Url})")
                .Build());

            return;
        }

        await response.ModifyResponseAsync(x => x.Embed = _embeds.Builder()
            .WithLyricsInformation(result.Lyrics)
            .Build());
    }

    [Command("np")]
    [Summary("Shows the track currently playing.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    public async Task NowPlayingAsync()
    {
        var queue = await _music.GetQueueAsync(Context.Guild!);

        if (queue.CurrentTrack is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithTrackStatusInformation(queue)
            .Build());
    }

    [Command("queue")]
    [Alias("q")]
    [Summary("Displays the queue.")]
    [RequireContext(ContextType.Guild, ErrorMessage = GuildRequiredErrorMessage)]
    public async Task QueueAsync()
    {
        var queue = await _music.GetQueueAsync(Context.Guild!);

        if (queue.Length == 0)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithQueueInformation(queue)
            .Build());
    }
}