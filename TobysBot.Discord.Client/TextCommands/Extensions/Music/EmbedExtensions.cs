using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using TobysBot.Discord.Audio;

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
    
    public static Embed BuildTrackStatusEmbed(this EmbedBuilder embed, ITrackStatus status)
    {
        var track = status.CurrentTrack;

        return embed
            .WithDescription($"[{track.Title}]({track.Url}) \n" +
                             $"{GetProgress(status.Position, status.Duration)} \n" +
                             $"{status.Position.ToTimeString()} / {status.Duration.ToTimeString()}")
            .WithContext(EmbedContext.Information)
            .Build();
    }

    public static Embed BuildQueueEmbed(this EmbedBuilder embed, IQueueStatus queue, ITrackStatus currentTrack)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("**Queue**");

        int i = 1;

        foreach (var track in queue.Previous())
        {
            sb.AppendLine($"**{i++}.** [{track.Title}]({track.Url})" +
                                 $" `{track.Duration.ToTimeString()}`");
        }

        if (currentTrack is null)
        {
            sb.AppendLine($"**--** No track playing.");

            if (queue.Next().Any())
            {
                sb.AppendLine($"**{i++}.** [{queue.CurrentTrack.Title}]({queue.CurrentTrack.Url})" +
                              $" `{queue.CurrentTrack.Duration.ToTimeString()}`");
            }
        }
        else
        {
            sb.AppendLine($"**{i++}. ({currentTrack.ToString()})** [{queue.CurrentTrack.Title}]({queue.CurrentTrack.Url})" +
                          $" `{currentTrack.Position.ToTimeString()}`/`{currentTrack.Duration.ToTimeString()}`");
        }
        
        foreach (var track in queue.Next())
        {
            sb.AppendLine($"**{i++}.** [{track.Title}]({track.Url})" +
                          $" `{track.Duration.ToTimeString()}`");
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
            .WithDescription($"Now playing [{track.Title}[({track.Url}) from [{playlist.Title}]({playlist.Url})")
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