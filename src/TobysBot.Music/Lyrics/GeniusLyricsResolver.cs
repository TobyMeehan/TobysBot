using Victoria.Resolvers;

namespace TobysBot.Music.Lyrics;

public class GeniusLyricsResolver : VictoriaBaseLyricsResolver
{
    protected override ValueTask<string> SearchAsync(string artist, string title)
    {
        return LyricsResolver.SearchGeniusAsync(artist, title);
    }

    protected override IProvider Provider => new Provider("Genius", "https://genius.com");

    public override int Priority => 0;
}