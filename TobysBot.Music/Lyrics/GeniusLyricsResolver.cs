using Victoria.Resolvers;

namespace TobysBot.Music.Lyrics;

public class GeniusLyricsResolver : ILyricsResolver
{
    private readonly Provider _provider = new("Genius", "https://genius.com");
    
    public async Task<ILyricsResult> TryResolveAsync(ITrack track)
    {
        var ly = await LyricsResolver.SearchGeniusAsync(null, track.Title);

        if (string.IsNullOrWhiteSpace(ly))
        {
            return new LyricsResult();
        }

        var lyrics = VictoriaLyrics.Parse(_provider, track.Title, ly);

        return new LyricsResult(lyrics);
    }

    public int Priority => 0;
}