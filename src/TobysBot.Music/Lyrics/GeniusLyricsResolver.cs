using Victoria.Resolvers;

namespace TobysBot.Music.Lyrics;

public class GeniusLyricsResolver : ILyricsResolver
{
    private readonly Provider _provider = new("Genius", "https://genius.com");
    
    public async Task<ILyricsResult> TryResolveAsync(ITrack track)
    {
        string lyricsString = await LyricsResolver.SearchGeniusAsync(track.Author, track.Title);

        if (string.IsNullOrWhiteSpace(lyricsString))
        {
            return new LyricsResult();
        }

        var lyrics = VictoriaLyrics.Parse(_provider, track, lyricsString);

        return new LyricsResult(lyrics);
    }

    public int Priority => 0;
}