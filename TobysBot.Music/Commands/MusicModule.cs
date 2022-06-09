using System.Globalization;
using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Commands.Response;
using TobysBot.Extensions;
using TobysBot.Music.Extensions;
using TobysBot.Music.Search.Result;
using TobysBot.Voice;
using TobysBot.Voice.Commands;
using TobysBot.Voice.Status;

namespace TobysBot.Music.Commands;

public class MusicModule : VoiceCommandModuleBase
{
    private readonly EmbedService _embeds;
    private readonly ISearchService _search;
    private readonly IMusicService _music;

    private IEmote PauseEmote => new Emoji("⏸");
    private IEmote PlayEmote => new Emoji("▶");
    private IEmote StopEmote => new Emoji("⏹");
    private IEmote ClearEmote => new Emoji("⏏");
    private IEmote FastForwardEmote => new Emoji("⏩");
    private IEmote RewindEmote => new Emoji("⏪");
    private IEmote ShuffleEmote => new Emoji("🔀");
    private IEmote SkipEmote => new Emoji("⏭");
    private IEmote BackEmote => new Emoji("⏮");
    private IEmote MoveEmote => new Emoji("↔");
    private IEmote RemoveEmote => new Emoji("⤴");

    public MusicModule(IVoiceService voiceService, EmbedService embeds, ISearchService search, IMusicService music) :
        base(voiceService, embeds)
    {
        _embeds = embeds;
        _search = search;
        _music = music;
    }

    [Command("play", RunMode = RunMode.Async)]
    [Alias("p")]
    [Summary("Loads query and adds it to the queue.")]
    public async Task PlayAsync(
        [Summary("Url or search for track to play.")] [Remainder]
        string query = null)
    {
        if (!await EnsureUserInVoiceAsync(required: false,
                sameChannel: true)) // If in a voice channel, must be the same
        {
            return;
        }

        ISocketResponse response;

        try
        {
            response = await Response.DeferAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return;
        }

        var search = query is null
            ? await _search.LoadAttachmentsAsync(Context.Message)
            : await _search.SearchAsync(query);

        switch (search)
        {
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

        await JoinVoiceChannelAsync(moveChannel: false);

        switch (search)
        {
            case TrackResult track:
                try
                {
                    await _music.EnqueueAsync(Context.Guild, track.Track);
                }
                catch (Exception e)
                {
                    await response.ModifyResponseAsync(x => x.Content = e.Message);
                }

                await response.ModifyResponseAsync(x =>
                {
                    x.Embed = _embeds.Builder()
                        .WithQueueTrackAction(track.Track)
                        .Build();
                });

                break;

            case PlaylistResult playlist:
                await _music.EnqueueAsync(Context.Guild, playlist.Playlist.Tracks);

                await response.ModifyResponseAsync(x =>
                {
                    x.Embed = _embeds.Builder()
                        .WithQueuePlaylistAction(playlist.Playlist)
                        .Build();
                });

                break;
        }
    }

    [Command("unpause")]
    [Alias("resume")]
    [Summary("Resumes playback.")]
    public async Task ResumeAsync()
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

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

        await _music.ResumeAsync(Context.Guild);

        await Response.ReactAsync(PlayEmote);
    }

    [Command("pause")]
    [Summary("Pauses playback.")]
    public async Task PauseAsync()
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

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

        await _music.PauseAsync(Context.Guild);

        await Response.ReactAsync(PauseEmote);
    }

    [Command("seek")]
    [Summary("Skips to the timestamp in the current track.")]
    public async Task SeekAsync(
        [Summary("Timestamp in the current track to skip to.")]
        string timestamp)
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

        if (Status is not PlayingStatus)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        string[] formats = { @"%h\:%m\:%s", @"%m\:%s" };

        if (!TimeSpan.TryParseExact(timestamp, formats, null, TimeSpanStyles.None, out var timeSpan))
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithCannotParseTimestampError()
                .Build());

            return;
        }

        var track = await _music.GetTrackAsync(Context.Guild);

        if (timeSpan > track.Duration)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithTimestampTooLongError()
                .Build());

            return;
        }

        await _music.SeekAsync(Context.Guild, timeSpan);

        await Response.ReactAsync(FastForwardEmote);
    }

    [Command("fastforward")]
    [Alias("ff")]
    [Summary("Fast forwards the track by the specified amount.")]
    public async Task FastForwardAsync(
        [Summary("Number of seconds to fast forward by.")]
        int seconds = 10)
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

        if (Status is not PlayingStatus)
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

        var track = await _music.GetTrackAsync(Context.Guild);
        var timeSpan = track.Position + TimeSpan.FromSeconds(seconds);

        if (timeSpan > track.Duration)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithFastForwardTooFarError()
                .Build());

            return;
        }

        await _music.SeekAsync(Context.Guild, timeSpan);

        await Response.ReactAsync(FastForwardEmote);
    }

    [Command("rewind")]
    [Alias("rw")]
    [Summary("Rewinds the track by the specified amount.")]
    public async Task RewindAsync(
        [Summary("Number of seconds to rewind by.")]
        int seconds = 10)
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

        if (Status is not PlayingStatus)
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

        var track = await _music.GetTrackAsync(Context.Guild);
        var timeSpan = track.Position - TimeSpan.FromSeconds(seconds);

        if (timeSpan < TimeSpan.Zero)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithRewindTooFarError()
                .Build());

            return;
        }

        await _music.SeekAsync(Context.Guild, timeSpan);

        await Response.ReactAsync(RewindEmote);
    }

    [Command("stop")]
    [Summary("Stops playback and returns to the start of the queue.")]
    public async Task StopAsync()
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

        await _music.StopAsync(Context.Guild);

        await Response.ReactAsync(StopEmote);
    }

    [Command("skip")]
    [Summary("Skips to the next track.")]
    public async Task SkipAsync()
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

        await _music.SkipAsync(Context.Guild);

        await Response.ReactAsync(SkipEmote);
    }

    [Command("back")]
    [Alias("previous")]
    [Summary("Skips to the previous track.")]
    public async Task BackAsync()
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

        var queue = await _music.GetQueueAsync(Context.Guild);

        if (!queue.Previous.Any())
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNoPreviousTrackError()
                .Build());

            return;
        }

        await _music.BackAsync(Context.Guild);

        await Response.ReactAsync(BackEmote);
    }

    [Command("jump")]
    [Summary("Jumps to the specified track.")]
    public async Task JumpAsync(
        [Summary("Position to jump to.")] int track)
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

        var queue = await _music.GetQueueAsync(Context.Guild);

        if (track < 1 || track > queue.Length)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithPositionOutOfRangeError()
                .Build());

            return;
        }

        await _music.JumpAsync(Context.Guild, track);

        await Response.ReactAsync(SkipEmote);
    }

    [Command("clear")]
    [Summary("Clears the queue.")]
    public async Task ClearAsync()
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

        await _music.ClearAsync(Context.Guild);

        await Response.ReactAsync(ClearEmote);
    }

    [Command("loop")]
    [Summary("Toggles looping for the track or queue.")]
    public async Task LoopAsync(
        [Summary("Loop over track or queue.")] LoopChoice mode = LoopChoice.Toggle)
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

        var queue = await _music.GetQueueAsync(Context.Guild);

        if (queue is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());

            return;
        }

        ILoopSetting setting;

        switch (mode)
        {
            case LoopChoice.Off:
            case LoopChoice.Toggle when queue.Loop is EnabledLoopSetting:
            case LoopChoice.Track when queue.Loop is TrackLoopSetting:
            case LoopChoice.Queue when queue.Loop is QueueLoopSetting:
                setting = new DisabledLoopSetting();
                break;

            case LoopChoice.Track:
            case LoopChoice.Toggle when queue.CurrentTrack is not null && !queue.Next.Any():
                setting = new TrackLoopSetting();
                break;

            case LoopChoice.Queue:
            case LoopChoice.Toggle:
                setting = new QueueLoopSetting();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "Invalid loop mode.");
        }

        await _music.SetLoopAsync(Context.Guild, setting);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithLoopAction(setting)
            .Build());
    }

    [Command("shuffle")]
    [Summary("Toggle shuffle mode.")]
    public async Task ShuffleAsync()
    {
        if (!await EnsureUserInVoiceAsync(sameChannel: true))
        {
            return;
        }

        var queue = await _music.GetQueueAsync(Context.Guild);

        await _music.SetShuffleAsync(Context.Guild, !queue.Shuffle);
        
        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithShuffleAction(!queue.Shuffle)
            .Build());
    }

    [Command("np")]
    [Summary("Shows the track currently playing.")]
    public async Task NowPlayingAsync()
    {
        var queue = await _music.GetQueueAsync(Context.Guild);

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
    public async Task QueueAsync()
    {
        var queue = await _music.GetQueueAsync(Context.Guild);

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