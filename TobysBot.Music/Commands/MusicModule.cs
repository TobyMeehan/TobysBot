using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Music.Extensions;
using TobysBot.Music.Search.Result;
using TobysBot.Voice;
using TobysBot.Voice.Commands;

namespace TobysBot.Music.Commands;

public class MusicModule : VoiceCommandModuleBase
{
    private readonly EmbedService _embeds;
    private readonly ISearchService _search;
    private readonly IMusicService _music;

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

        var response = await Response.DeferAsync();

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
}