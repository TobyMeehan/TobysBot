using Discord;

namespace TobysBot.Discord.Audio.Status
{
    public class NotPlayingStatus : IConnectedStatus
    {
        public NotPlayingStatus(IVoiceChannel channel, ITextChannel textChannel)
        {
            VoiceChannel = channel;
            TextChannel = textChannel;
        }

        public IVoiceChannel VoiceChannel { get; }
        public ITextChannel TextChannel { get; }
    }
}