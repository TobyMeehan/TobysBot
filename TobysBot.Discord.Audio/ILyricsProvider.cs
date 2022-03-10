using System.Threading.Tasks;

namespace TobysBot.Discord.Audio;

public interface ILyricsProvider
{
    Task<string> GetLyricsAsync(ITrack track);
}