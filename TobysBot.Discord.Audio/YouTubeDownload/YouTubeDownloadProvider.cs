using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace TobysBot.Discord.Audio.YouTubeDownload;

public class YouTubeDownloadProvider : IDownloadProvider
{
    private readonly YoutubeClient _client;

    public YouTubeDownloadProvider(YoutubeClient client)
    {
        _client = client;
    }
    
    public async Task<Stream> GetDownloadAsync(ITrack track)
    {
        var streamManifest = await _client.Videos.Streams.GetManifestAsync(track.SourceUrl);

        var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

        var stream = await _client.Videos.Streams.GetAsync(streamInfo);

        return stream;
    }
}