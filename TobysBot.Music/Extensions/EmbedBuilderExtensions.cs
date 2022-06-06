using Discord;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Music.Configuration;
using TobysBot.Music.Search.Result;

namespace TobysBot.Music.Extensions;

public static class EmbedBuilderExtensions
{
    public static EmbedBuilder WithPlayTrackAction(this EmbedBuilder embed, ITrack track)
    {
        return embed
            .WithContext(EmbedContext.Action)
            .WithDescription($"Now playing [{track.Title}]({track.Url})");
    }

    public static EmbedBuilder WithQueueTrackAction(this EmbedBuilder embed, ITrack track)
    {
        return embed
            .WithContext(EmbedContext.Action)
            .WithDescription($"Queued [{track.Title}]({track.Url})");
    }

    public static EmbedBuilder WithQueuePlaylistAction(this EmbedBuilder embed, IPlaylist playlist)
    {
        return embed
            .WithContext(EmbedContext.Action)
            .WithDescription($"Queued {playlist.Tracks.Count()} tracks from [{playlist.Title}]({playlist.Url})");
    }

    public static EmbedBuilder WithNotFoundError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<MusicOptions>().Embeds.NotFoundErrorDescription);
    }

    public static EmbedBuilder WithLoadFailedError(this EmbedBuilder embed, LoadFailedSearchResult result)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(result.Reason);
    }
}