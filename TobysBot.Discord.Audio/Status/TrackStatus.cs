using Discord;

namespace TobysBot.Discord.Audio.Status
{
    public class TrackStatus : ITrackStatus
    {
        public TrackStatus(IActiveTrack currentTrack, IVoiceChannel channel, bool isPaused)
        {
            CurrentTrack = currentTrack;
            Channel = channel;
            IsPaused = isPaused;
        }
        
        public virtual IActiveTrack CurrentTrack { get; }
        
        public bool IsPaused { get; }

        public IVoiceChannel Channel { get; }
    }
}