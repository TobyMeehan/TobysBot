using System.Text;
using Discord;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Music.Configuration;
using TobysBot.Music.Search.Result;
using TobysBot.Voice.Status;

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

    public static EmbedBuilder WithNotPlayingError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<MusicOptions>().Embeds.NotPlayingErrorDescription);
    }

    public static EmbedBuilder WithAlreadyPlayingError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<MusicOptions>().Embeds.AlreadyPlayingErrorDescription);
    }

    public static EmbedBuilder WithAlreadyPausedError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<MusicOptions>().Embeds.AlreadyPausedErrorDescription);
    }

    public static EmbedBuilder WithCannotParseTimestampError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<MusicOptions>().Embeds.CannotParseTimestampErrorDescription);
    }

    public static EmbedBuilder WithTimestampTooLongError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<MusicOptions>().Embeds.TimestampTooLongErrorDescription);
    }

    public static EmbedBuilder WithFastForwardTooFarError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<MusicOptions>().Embeds.FastForwardTooFarErrorDescription);
    }

    public static EmbedBuilder WithRewindTooFarError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<MusicOptions>().Embeds.RewindTooFarErrorDescription);
    }

    private static string GetProgressBar(TimeSpan position, TimeSpan duration)
    {
        var fraction = position.Ticks / (double)duration.Ticks;
        fraction *= 100d;

        var percent = (int)fraction;
        percent /= 4;

        var progress = new string('▬', 25);

        return progress.Remove(percent, 1).Insert(percent, "⬤");
    }
    
    public static EmbedBuilder WithTrackStatusInformation(this EmbedBuilder embed, PlayingStatus status, IQueue queue)
    {
        var track = queue.CurrentTrack;

        return embed
            .WithContext(EmbedContext.Information)
            .WithDescription($"[{track.Title}]({track.Url}) \n" +
                             
                             $"`{track.Position.ToTimeStamp()}` " +
                             $"{GetProgressBar(track.Position, track.Duration)} " +
                             $"`{track.Duration.ToTimeStamp()}` \n" +
                             
                             $"{(status.IsPaused ? "⏸" : "▶")}" +
                             $"{(queue.Loop is TrackLoopSetting ? " 🔂" : "")}" +
                             $"{(queue.Loop is QueueLoopSetting ? " 🔁" : "")}" +
                             $"{(queue.Shuffle ? " 🔀" : "")}" +
                             
                             $"");
    }

    public static EmbedBuilder WithQueueInformation(this EmbedBuilder embed, IQueue queue, IPlayerStatus status)
    {
        var previous = new Queue<ITrack>(queue.Previous.Reverse());
        var next = new Queue<ITrack>(queue.Next);
        var current = queue.CurrentTrack;

        var currentPosition = previous.Count;

        var sb = new StringBuilder();

        if (current is null)
        {
            sb.AppendLine("**--** No track playing.");
        }
        else
        {
            sb.Append($"**{currentPosition + 1}. ");
            sb.Append('(');

            switch (status)
            {
                case PlayingStatus {IsPaused: true}:
                    sb.Append('⏸');
                    break;
                case PlayingStatus {IsPaused: false}:
                    sb.Append('▶');
                    break;
                case IConnectedStatus:
                    sb.Append('⏹');
                    break;
            }

            sb.Append($"{(queue.Loop is TrackLoopSetting ? " 🔂" : "")})** ");
            sb.Append($"**[{current.Title}]({current.Url})** ");
            sb.Append($"`{current.Position.ToTimeStamp()}`/`{current.Duration.ToTimeStamp()}`");
            sb.AppendLine();
        }

        var i = 0;

        while (sb.Length < 1900 && (previous.Any() || next.Any()))
        {
            if (previous.TryDequeue(out var previousTrack))
            {
                sb.PrependLine($"**{currentPosition - i}.** " +
                               $"[{previousTrack.Title}]({previousTrack.Url})" +
                               $" `{previousTrack.Duration.ToTimeStamp()}`");
            }

            if (next.TryDequeue(out var nextTrack))
            {
                sb.AppendLine($"**{currentPosition + i + 2}.** " +
                              $"[{nextTrack.Title}]({nextTrack.Url})" +
                              $" `{nextTrack.Duration.ToTimeStamp()}`");
            }

            i++;
        }

        sb.AppendLine();
        
        if (previous.Any() || next.Any())
        {
            
            sb.AppendLine($"{previous.Count + next.Count} other tracks in the queue.");
        }
        else
        {
            sb.AppendLine("This is the end of the queue.");
        }

        sb.AppendLine("Use **/play** to add more");
        
        switch (queue.Loop)
        {
            case QueueLoopSetting:
                sb.AppendLine();
                sb.AppendLine("🔁 Looping the **queue**.");
                break;
            case TrackLoopSetting:
                sb.AppendLine();
                sb.AppendLine("🔂 Looping the **current track**.");
                break;
        }

        if (queue.Shuffle)
        {
            sb.AppendLine();
            sb.AppendLine("🔀 Shuffle mode is **enabled**.");
        }

        return embed
            .WithContext(EmbedContext.Information)
            .WithDescription(sb.ToString());
    }
}