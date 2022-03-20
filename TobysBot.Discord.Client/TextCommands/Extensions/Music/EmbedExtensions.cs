using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using TobysBot.Discord.Audio;
using TobysBot.Discord.Client.Extensions;

namespace TobysBot.Discord.Client.TextCommands.Extensions.Music;

public static class EmbedExtensions
{
    private static string GetProgress(TimeSpan position, TimeSpan duration)
    {
        double fraction = (double)position.Ticks / (double)duration.Ticks;
        fraction *= 100d;

        int percent = (int)fraction;
        percent /= 4;

        string progress = new string('▬', 25);

        return progress.Remove(percent, 1).Insert(percent, "⬤");
    }
    
    public static Embed BuildTrackStatusEmbed(this EmbedBuilder embed, ITrackStatus status, IQueueStatus queueStatus)
    {
        var track = status.CurrentTrack;

        return embed
            .WithDescription($"[{track.Title}]({track.Url}) \n" +
                             $"`{track.Position.ToTimeString()}` " +
                             $"{GetProgress(track.Position, track.Duration)} " +
                             $"`{track.Duration.ToTimeString()}` \n" +
                             $"{(status is PausedStatus ? "⏸" : "▶")}" +
                             $"{(queueStatus.LoopEnabled is TrackLoopSetting ? " 🔂": "")}" +
                             $"{(queueStatus.LoopEnabled is QueueLoopSetting ? " 🔁" : "")}" +
                             $"{(queueStatus.ShuffleEnabled is EnabledShuffleSetting ? " 🔀" : "")}" +
                             $"")
            .WithContext(EmbedContext.Information)
            .Build();
    }
    
    public static Embed BuildQueueEmbed(this EmbedBuilder embed, IQueueStatus queue, ITrackStatus trackStatus)
    {
        var previous = queue.Previous().Reverse().ToList();
        var next = queue.Next().ToList();
        var current = trackStatus?.CurrentTrack;
        var sb = new StringBuilder();
        var i = 0;
        var totalInQueue = previous.Count + next.Count;
        var elementsAdded = 1;

        if (trackStatus is null)
        {
            sb.AppendLine("**--** No track playing.");
        }
        else
        {
            sb.AppendLine($"**{previous.Count + 1}. " +
                          $"({(trackStatus is PausedStatus ? "⏸" : "▶")}" +
                          $"{(queue.LoopEnabled is TrackLoopSetting ? " 🔂": "")})** " +
                          $"[{current.Title}]({current.Url}) " +
                          $"`{current.Position.ToTimeString()}`/`{current.Duration.ToTimeString()}`");
        }
        
        while (sb.Length < 1900 && i < Math.Max(previous.Count, next.Count))
        {
            if (previous.ElementAtOrDefault(i) is not null)
            {
                sb.PrependLine($"**{previous.Count - i}.** [{previous[i].Title}]({previous[i].Url})" +
                              $" `{previous[i].Duration.ToTimeString()}`");

                elementsAdded++;
            }
            
            if (next.ElementAtOrDefault(i) is not null)
            {
                sb.AppendLine($"**{previous.Count + i + 2}.** [{next[i].Title}]({next[i].Url})" +
                              $" `{next[i].Duration.ToTimeString()}`");

                elementsAdded++;
            }

            i++;
        }

        if (elementsAdded < totalInQueue)
        {
            sb.AppendLine();
            sb.AppendLine($"`{totalInQueue - elementsAdded + 1} other tracks in queue.`");
        }

        switch (queue.LoopEnabled)
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

        if (queue.ShuffleEnabled is EnabledShuffleSetting)
        {
            sb.AppendLine();
            sb.AppendLine("🔀 Shuffle mode is **enabled**.");
        }
        
        return embed
            .WithContext(EmbedContext.Information)
            .WithDescription(sb.ToString())
            .Build();
    }

    public static Embed BuildPlayTrackEmbed(this EmbedBuilder embed, ITrack track)
    {
        return embed
            .WithDescription($"Now playing [{track.Title}]({track.Url})")
            .WithContext(EmbedContext.Action)
            .Build();
    }

    public static Embed BuildPlayPlaylistEmbed(this EmbedBuilder embed, IPlaylist playlist)
    {
        var track = playlist.First();

        return embed
            .WithDescription($"Now playing [{track.Title}]({track.Url}) " +
                             $"and {playlist.Count() - 1} others " +
                             $"from [{playlist.Title}]({playlist.Url})")
            .WithContext(EmbedContext.Action)
            .Build();
    }

    public static Embed BuildQueueTrackEmbed(this EmbedBuilder embed, ITrack track)
    {
        return embed
            .WithDescription($"Queued [{track.Title}]({track.Url})")
            .WithContext(EmbedContext.Action)
            .Build();
    }

    public static Embed BuildQueuePlaylistEmbed(this EmbedBuilder embed, IPlaylist playlist)
    {
        return embed
            .WithDescription($"Queued {playlist.Count()} tracks from [{playlist.Title}]({playlist.Url})")
            .WithContext(EmbedContext.Action)
            .Build();
    }

    public static Embed BuildLoopTrackEmbed(this EmbedBuilder embed)
    {
        return embed
            .WithDescription($"Looping the **current track**.")
            .WithContext(EmbedContext.Action)
            .Build();
    }

    public static Embed BuildLoopQueueEmbed(this EmbedBuilder embed)
    {
        return embed
            .WithDescription($"Looping the **queue**.")
            .WithContext(EmbedContext.Action)
            .Build();
    }

    public static Embed BuildLoopDisabledEmbed(this EmbedBuilder embed)
    {
        return embed
            .WithDescription($"Looping is **disabled**.")
            .WithContext(EmbedContext.Action)
            .Build();
    }

    public static Embed BuildLyricsEmbed(this EmbedBuilder embed, ITrack track, string lyrics)
    {
        var lines = lyrics.Split("\n");
        
        StringBuilder sb = new();

        sb.AppendLine($"**{track.Title}**");
        
        foreach (var line in lines)
        {
            if (line.Contains('['))
            {
                sb.AppendLine();
            }

            if (sb.Length is > 1900 and < 2000)
            {
                sb.AppendLine("`...`");
                break;
            }

            sb.AppendLine(line.Replace("[", "*").Replace("]", "*"));
        }

        return embed
            .WithDescription(sb.ToString())
            .WithContext(EmbedContext.Information)
            .Build();
    }

    public static Embed BuildTrackNotFoundEmbed(this EmbedBuilder embed, string query)
    {
        return embed
            .WithDescription($"No results for {query} :(")
            .WithContext(EmbedContext.Error)
            .Build();
    }

    public static Embed BuildJoinVoiceEmbed(this EmbedBuilder embed)
    {
        return embed
            .WithDescription("Join the voice channel you square.")
            .WithContext(EmbedContext.Error)
            .Build();
    }

    public static Embed BuildJoinSameVoiceEmbed(this EmbedBuilder embed)
    {
        return embed
            .WithDescription("We need to be in the same voice channel to do that.")
            .WithContext(EmbedContext.Error)
            .Build();
    }

    public static Embed BuildNotPlayingEmbed(this EmbedBuilder embed)
    {
        return embed
            .WithDescription("I'm not currently playing anything.")
            .WithContext(EmbedContext.Error)
            .Build();
    }
    
    
}