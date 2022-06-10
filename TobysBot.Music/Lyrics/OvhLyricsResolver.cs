using Victoria.Resolvers;

namespace TobysBot.Music.Lyrics;

public class OvhLyricsResolver : ILyricsResolver
{
    private readonly Provider _provider = new("OVH", "https://lyrics.ovh");
    
    public async Task<ILyricsResult> TryResolveAsync(ITrack track)
    {
        var ly = await LyricsResolver.SearchOvhAsync(null, track.Title);

        if (string.IsNullOrWhiteSpace(ly))
        {
            return new LyricsResult();
        }

        var lyrics = VictoriaLyrics.Parse(_provider, track.Title, ly);

        return new LyricsResult(lyrics);
    }

    public int Priority => 50;
}