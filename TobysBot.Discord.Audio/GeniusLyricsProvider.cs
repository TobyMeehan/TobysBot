using System.Threading.Tasks;
using TobysBot.Discord.Audio.Extensions;
using Victoria;

namespace TobysBot.Discord.Audio;

public class GeniusLyricsProvider : ILyricsProvider
{
    private readonly LavaNode _node;

    public GeniusLyricsProvider(LavaNode node)
    {
        _node = node;
    }
    
    public async Task<string> GetLyricsAsync(ITrack track)
    {
        var lavaTrack = await _node.LoadTrackAsync(track);

        return await lavaTrack.FetchLyricsFromGeniusAsync();
    }
}