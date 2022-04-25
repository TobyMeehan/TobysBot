using System.IO;
using System.Threading.Tasks;

namespace TobysBot.Discord.Audio;

public interface IDownloadProvider
{
    Task<Stream> GetDownloadAsync(ITrack track);
}