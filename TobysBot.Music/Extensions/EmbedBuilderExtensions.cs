using System.Text;
using Discord;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Music.Configuration;
using TobysBot.Music.Data;
using TobysBot.Music.Lyrics;
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

    public static EmbedBuilder WithQueueSavedQueueAction(this EmbedBuilder embed, ISavedQueue savedQueue)
    {
        return embed
            .WithContext(EmbedContext.Action)
            .WithDescription($"Queued {savedQueue.Tracks.Count()} tracks from **{savedQueue.Name}**");
    }

    public static EmbedBuilder WithLoopAction(this EmbedBuilder embed, ILoopSetting setting)
    {
        string description = setting switch
        {
            DisabledLoopSetting => "Looping is **disabled**.",
            EnabledLoopSetting => $"Looping the **{(setting is TrackLoopSetting ? "current track" : "")}{(setting is QueueLoopSetting ? "queue" : "")}**.",
            _ => throw new ArgumentOutOfRangeException(nameof(setting), "Unexpected loop setting.")
        };

        return embed
            .WithContext(EmbedContext.Action)
            .WithDescription(description);
    }

    public static EmbedBuilder WithShuffleAction(this EmbedBuilder embed, bool shuffle)
    {
        return embed
            .WithContext(EmbedContext.Action)
            .WithDescription($"Shuffle mode is **{(shuffle ? "enabled" : "disabled")}**.");
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

    public static EmbedBuilder WithNoPreviousTrackError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<MusicOptions>().Embeds.NoPreviousTrackErrorDescription);
    }

    public static EmbedBuilder WithPositionOutOfRangeError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription(x => x.Service.Options<MusicOptions>().Embeds.PositionOutOfRangeErrorDescription);
    }

    public static EmbedBuilder WithSavedQueueNotFoundError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription("You have no saved queues with that name.");
    }

    public static EmbedBuilder WithLyricsInformation(this EmbedBuilder embed, ILyrics lyrics)
    {
        var sb = new StringBuilder();

        foreach (var line in lyrics.Lines)
        {
            if (sb.Length is > 1900 and < 2000)
            {
                sb.AppendLine();
                sb.AppendLine("...");
                break;
            }
            
            if (line is Header)
            {
                sb.AppendLine();
                sb.AppendLine($"*{line.Content}*");
                continue;
            }

            sb.AppendLine(line.Content);
        }

        return embed
            .WithTitle(lyrics.Track.Title)
            .WithUrl(lyrics.Track.Url)
            .WithDescription(sb.ToString())
            .WithFooter($"Lyrics provided by {lyrics.Provider.Name}")
            .WithContext(EmbedContext.Information);
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
    
    public static EmbedBuilder WithTrackStatusInformation(this EmbedBuilder embed, IQueue queue)
    {
        var track = queue.CurrentTrack;

        var sb = new StringBuilder();

        sb.AppendLine($"[{track.Title}]({track.Url})");

        sb.Append($"`{track.Position.ToTimeStamp()}`");
        sb.Append(' ');
        sb.Append(GetProgressBar(track.Position, track.Duration));
        sb.Append(' ');
        sb.Append($"`{track.Duration.ToTimeStamp()}`");

        sb.AppendLine();

        sb.Append(track.Status switch
        {
            ActiveTrackStatus.Playing => "▶",
            ActiveTrackStatus.Paused => "⏸",
            ActiveTrackStatus.Stopped => "⏹",
            _ => ""
        });

        sb.Append(' ');
        
        sb.Append(queue.Loop switch
        {
            TrackLoopSetting => "🔂",
            QueueLoopSetting => "🔁",
            _ => ""
        });

        if (queue.Shuffle)
        {
            sb.Append(' ');
            sb.Append("🔀"); 
        }
        
        return embed
            .WithContext(EmbedContext.Information)
            .WithDescription(sb.ToString());
    }

    public static EmbedBuilder WithQueueInformation(this EmbedBuilder embed, IQueue queue)
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

            switch (queue.CurrentTrack.Status)
            {
                case ActiveTrackStatus.Paused:
                    sb.Append('⏸');
                    break;
                case ActiveTrackStatus.Playing:
                    sb.Append('▶');
                    break;
                case ActiveTrackStatus.Stopped:
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

    public static EmbedBuilder WithSavedQueueListInformation(this EmbedBuilder embed, IUser user, IEnumerable<ISavedQueue> savedQueues)
    {
        foreach (var queue in savedQueues)
        {
            embed.AddField(field =>
            {
                field
                    .WithName($"{queue.Name}")
                    .WithValue($"{queue.Tracks.Count()} tracks - total duration {queue.Tracks.Sum(x => x.Duration).ToTimeStamp()}");
            });
        }
        
        return embed
            .WithContext(EmbedContext.Information)
            .WithDescription($"**Saved Queues for {user.Mention}**");
    }
}