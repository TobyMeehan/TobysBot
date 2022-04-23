using Discord;

namespace TobysBot.Discord.Audio.Status
{
    public class NotPlayingStatus : IConnectedStatus
    {
        public NotPlayingStatus(IVoiceChannel channel)
        {
            Channel = channel;
        }

        public IVoiceChannel Channel { get; }
    }
}