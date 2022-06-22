using Victoria.Resolvers;

namespace TobysBot.Music.Lyrics;

public class OvhLyricsResolver : VictoriaBaseLyricsResolver
{
    protected override ValueTask<string> SearchAsync(string artist, string title)
    {
        return LyricsResolver.SearchOvhAsync(artist, title);
    }

    protected override IProvider Provider => new Provider("OVH", "https://lyrics.ovh");

    public override int Priority => 50;
}