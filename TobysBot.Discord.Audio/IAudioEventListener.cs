using System;
using System.Threading.Tasks;
using Discord;

namespace TobysBot.Discord.Audio
{
    public interface IAudioEventListener
    {
        Task OnTrackEnded(ITrack track, ITextChannel textChannel);

        Task OnTrackException(ITrack track, ITextChannel textChannel, string message);

        Task OnTrackStarted(ITrack track, ITextChannel textChannel);

        Task OnTrackStuck(ITrack track, ITextChannel textChannel, TimeSpan threshold);
    }
}