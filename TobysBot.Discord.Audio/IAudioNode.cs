using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TobysBot.Discord.Audio
{
    public interface IAudioNode
    {
        // Voice channel
        Task JoinAsync(IVoiceChannel channel, ITextChannel textChannel);
        Task LeaveAsync(IGuild guild);

        // Track controls
        Task<ITrack> PlayAsync(string query, IVoiceChannel channel);
        Task PauseAsync(IGuild guild);
        Task StopAsync(IGuild guild);

        // Queue
        Task<ITrack> SkipAsync(IGuild guild);
        Task ClearAsync(IGuild guild);


        // Information
        Task<ITrack> GetCurrentTrackAsync(IGuild guild);
        Task<IQueue> GetQueueAsync(IGuild guild);
    }
}
