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
    
    public MusicModule(IVoiceService voiceService, EmbedService embeds, ISearchService search, IMusicService music) : base(voiceService, embeds)
    {
        _embeds = embeds;
        _search = search;
        _music = music;
    }

    [Command("play", RunMode = RunMode.Async)]
    [Alias("p")]
    [Summary("Loads query and adds it to the queue.")]
    public async Task PlayAsync(
        [Summary("Url or search for track to play.")]
        [Remainder] string query = null)
    {
        if (!await EnsureUserInVoiceAsync(required: false, sameChannel: true)) // If in a voice channel, must be the same
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

    [Command("np")]
    [Summary("Shows the track currently playing.")]
    public async Task NowPlayingAsync()
    {
        if (Status is not PlayingStatus playingStatus)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());
            
            return;
        }

        var queue = await _music.GetQueueAsync(Context.Guild);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithTrackStatusInformation(playingStatus, queue)
            .Build());
    }

    [Command("queue")]
    [Alias("q")]
    [Summary("Displays the queue.")]
    public async Task QueueAsync()
    {
        if (Status is not IConnectedStatus)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithNotPlayingError()
                .Build());
            
            return;
        }

        var queue = await _music.GetQueueAsync(Context.Guild);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithQueueInformation(queue, Status)
            .Build());
    }
}