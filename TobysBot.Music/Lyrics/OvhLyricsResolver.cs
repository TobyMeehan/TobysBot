using Victoria.Resolvers;

namespace TobysBot.Music.Lyrics;

public class OvhLyricsResolver : ILyricsResolver
{
    private readonly Provider _provider = new("OVH", "https://lyrics.ovh");
    
    public async Task<ILyricsResult> TryResolveAsync(ITrack track)
    {
        var ly = await LyricsResolver.SearchOvhAsync(track.Author, track.Title);

        if (string.IsNullOrWhiteSpace(ly))
        {
            return new LyricsResult();
        }

        var lyrics = VictoriaLyrics.Parse(_provider, track, ly);

        return new LyricsResult(lyrics);
    }

    public int Priority => 50;
}