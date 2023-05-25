using System.Text;
using Discord;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Music.Configuration;
using TobysBot.Music.Lyrics;
using TobysBot.Music.Search.Result;
using System;

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
        var sb = new StringBuilder();

        switch (setting)
        {
            case DisabledLoopSetting:
                sb.AppendLine("Looping is **disabled**.");
                break;
            
            case EnabledLoopSetting:
                sb.Append("Looping the ");

                sb.Append(setting switch
                {
                    TrackLoopSetting => "**current track**",
                    QueueLoopSetting => "**queue**",
                    _ => throw new ArgumentOutOfRangeException(nameof(setting), "Unexpected loop setting.")
                });

                if (setting is TrackLoopSetting {Start: not null} start)
                {
                    sb.Append($" from **{start.Start.Value.ToTimeStamp()}**");
                }

                if (setting is TrackLoopSetting {End: not null} end)
                {
                    sb.Append($" to **{end.End.Value.ToTimeStamp()}**");
                }

                sb.AppendLine(".");
                break;
        }

        return embed
            .WithContext(EmbedContext.Action)
            .WithDescription(sb.ToString());
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
            .WithDescription("No results for that query.");
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
            .WithDescription("I'm not playing anything.");
    }

    public static EmbedBuilder WithAlreadyPlayingError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription("The track is already playing!");
    }

    public static EmbedBuilder WithAlreadyPausedError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription("The track is already paused!");
    }

    public static EmbedBuilder WithCannotParseTimestampError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription("Could not parse that timestamp.");
    }

    public static EmbedBuilder WithTimestampTooLongError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription("The track is not that long.");
    }

    public static EmbedBuilder WithFastForwardTooFarError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription("Can't fast forward beyond the track!");
    }

    public static EmbedBuilder WithRewindTooFarError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription("Can't rewind to before the track!");
    }

    public static EmbedBuilder WithNoPreviousTrackError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription("No previous track to jump to.");
    }

    public static EmbedBuilder WithPositionOutOfRangeError(this EmbedBuilder embed)
    {
        return embed
            .WithContext(EmbedContext.Error)
            .WithDescription("No track at that position.");
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
    
    private static string GetProgressBar(TimeSpan position, TimeSpan duration, ILoopSetting loopSetting)
    {
        double percentPosition = 100d * (position.Ticks / (double)duration.Ticks);
        double percentLoopStart =
            loopSetting is TrackLoopSetting {Start.TotalSeconds: > 0} s 
                ? 100d * (s.Start.Value.Ticks / (double)duration.Ticks) : -1;
        double percentLoopEnd = 
            loopSetting is TrackLoopSetting {End.TotalSeconds: > 0} e 
                ? 100d * (e.End.Value.Ticks / (double) duration.Ticks) : -1;

        string progress = new('▬', 25);

        int positionIndex = (int) (percentPosition / 4);
        int loopStartIndex = percentLoopStart > 0 ? Math.Min((int) (percentLoopStart / 4), positionIndex) : -1;
        int loopEndIndex = percentLoopEnd > 0 ? Math.Max((int) (percentLoopEnd / 4), loopStartIndex + 2) : -1;

        progress = progress.Remove(positionIndex, 1).Insert(positionIndex, "⬤");

        if (percentLoopStart > 0)
        {
            progress = progress.Insert(loopStartIndex > positionIndex ? positionIndex-1 : loopStartIndex, "|");
        }

        if (percentLoopEnd > 0)
        {
            progress = progress.Insert(loopEndIndex <= positionIndex ? positionIndex+1 : loopEndIndex, "|");
        }

        return progress;
    }
    
    public static EmbedBuilder WithTrackStatusInformation(this EmbedBuilder embed, IQueue queue)
    {
        if (queue.CurrentTrack is not { } track)
        {
            throw new NullReferenceException("Current track is null.");
        }

        var sb = new StringBuilder();

        sb.AppendLine($"[{Format.Sanitize(track.Title)}]({track.Url}) [{track.RequestedBy.Mention}]");

        sb.Append($"`{track.Position.ToTimeStamp()}`");
        sb.Append(' ');
        sb.Append(GetProgressBar(track.Position, track.Duration, queue.Loop));
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
            TrackLoopSetting { Start: not null, End: not null } t => $"{t.Start.Value.ToTimeStamp()} 🔂 {t.End.Value.ToTimeStamp()}",
            TrackLoopSetting { Start: not null } s => $"{s.Start.Value.ToTimeStamp()} 🔂 {track.Duration.ToTimeStamp()}",
            TrackLoopSetting => "🔂",
            EnabledLoopSetting => "🔁",
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

        int currentPosition = previous.Count;

        var sb = new StringBuilder();

        if (current is null)
        {
            sb.AppendLine("**--** No track playing.");
        }
        else
        {
            sb.Append($"**{currentPosition + 1}. ");
            sb.Append('(');

            switch (current.Status)
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
            sb.Append($"**[{Format.Sanitize(current.Title)}]({current.Url})** ");
            sb.Append($"[{current.RequestedBy.Mention}] ");
            sb.Append($"`{current.Position.ToTimeStamp()}`/`{current.Duration.ToTimeStamp()}`");
            sb.AppendLine();
        }

        int i = 0;

        while (sb.Length < 1900 && (previous.Any() || next.Any()))
        {
            if (previous.TryDequeue(out var previousTrack))
            {
                sb.PrependLine($"**{currentPosition - i}.** " +
                               $"[{previousTrack.Title}]({previousTrack.Url}) " +
                               $"[{previousTrack.RequestedBy.Mention}] " +
                               $"`{previousTrack.Duration.ToTimeStamp()}`");
            }

            if (next.TryDequeue(out var nextTrack))
            {
                sb.AppendLine($"**{currentPosition + i + 2}.** " +
                              $"[{nextTrack.Title}]({nextTrack.Url}) " +
                              $"[{nextTrack.RequestedBy.Mention}] " +
                              $"`{nextTrack.Duration.ToTimeStamp()}`");
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