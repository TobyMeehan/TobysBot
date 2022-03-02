using System.Threading.Tasks;

namespace TobysBot.Discord.Audio
{
    public interface IAudioSource
    {
        Task<IPlayable> SearchAsync(string query);
    }
}