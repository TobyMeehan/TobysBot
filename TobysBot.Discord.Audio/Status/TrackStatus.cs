using Discord;

namespace TobysBot.Discord.Audio.Status
{
    public class TrackStatus : ITrackStatus
    {
        public TrackStatus(IActiveTrack currentTrack, IVoiceChannel voiceChannel, ITextChannel textChannel, bool isPaused)
        {
            CurrentTrack = currentTrack;
            VoiceChannel = voiceChannel;
            IsPaused = isPaused;
            TextChannel = textChannel;
        }
        
        public virtual IActiveTrack CurrentTrack { get; }
        
        public bool IsPaused { get; }

        public IVoiceChannel VoiceChannel { get; }

        public ITextChannel TextChannel { get; }
    }
}