using System.Threading.Tasks;
using Discord;

namespace TobysBot.Discord.Audio
{
    public interface IAudioSource
    {
        Task<IPlayable> SearchAsync(string query);

        Task<IPlayable> LoadAttachmentsAsync(IMessage message);
    }
}