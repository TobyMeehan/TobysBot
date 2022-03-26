using System;
using System.Threading.Tasks;
using Discord;
using TobysBot.Discord.Audio;
using TobysBot.Discord.Client.TextCommands.Extensions;

namespace TobysBot.Discord.Client.Configuration;

public class AudioEventListener : IAudioEventListener
{
    public Task OnTrackEnded(ITrack track, ITextChannel textChannel)
    {
        return Task.CompletedTask;
    }

    public Task OnTrackException(ITrack track, ITextChannel textChannel, string message)
    {
        return Task.CompletedTask;
    }

    public async Task OnTrackStarted(ITrack track, ITextChannel textChannel)
    {
        await textChannel.SendMessageAsync(embed: new EmbedBuilder().BuildPlayTrackEmbed(track));
    }

    public Task OnTrackStuck(ITrack track, ITextChannel textChannel, TimeSpan threshold)
    {
        return Task.CompletedTask;
    }
}