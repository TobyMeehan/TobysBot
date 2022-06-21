using Victoria.Resolvers;

namespace TobysBot.Music.Lyrics;

public class OvhLyricsResolver : ILyricsResolver
{
    private readonly Provider _provider = new("OVH", "https://lyrics.ovh");
    
    public async Task<ILyricsResult> TryResolveAsync(ITrack track)
    {
        string lyricsString = await LyricsResolver.SearchOvhAsync(track.Author, track.Title);

        if (string.IsNullOrWhiteSpace(lyricsString))
        {
            return new LyricsResult();
        }

        var lyrics = VictoriaLyrics.Parse(_provider, track, lyricsString);

        return new LyricsResult(lyrics);
    }

    public int Priority => 50;
}