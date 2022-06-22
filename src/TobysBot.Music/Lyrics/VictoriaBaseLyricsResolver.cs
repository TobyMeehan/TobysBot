using System.Text.RegularExpressions;
using TobysBot.Music.Search;

namespace TobysBot.Music.Lyrics;

public abstract class VictoriaBaseLyricsResolver : ILyricsResolver
{
    private static (string Artist, string Title) GetArtistAndTitle(ITrack track)
    {
        if (track is IActiveTrack activeTrack)
        {
            return GetArtistAndTitle(activeTrack.InnerTrack);
        }

        var parenReg
            = new Regex(@"(\(.*?\))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var artistReg
            = new Regex(@"\w+.\w+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        if (track is SpotifyTrack spotifyTrack)
        {
            return (spotifyTrack.Author, parenReg.Replace(spotifyTrack.Title, string.Empty).Split('-')[0].Trim());
        }
        
        string title = parenReg.Replace(track.Title, string.Empty);
        title = title.Replace("&", "and");
        string[] titleSplit = title.Split('-');

        string[] artistSplit = track.Author.Split('-');
        if (titleSplit.Length == 1 && artistSplit.Length > 1)
        {
            return (artistSplit[0].Trim(), title.Trim());
        }

        string artist = artistReg.Match(titleSplit[0]).Value;
        if (artist.Equals(titleSplit[0], StringComparison.OrdinalIgnoreCase) ||
            artist.Equals(track.Author, StringComparison.OrdinalIgnoreCase))
        {
            return (titleSplit[0].Trim(), titleSplit[1].Trim());
        }

        return (artist, titleSplit[1].Trim());
    }
    
    public async Task<ILyricsResult> TryResolveAsync(ITrack track)
    {
        (string artist, string title) = GetArtistAndTitle(track);

        string lyricsString;
        try
        {
            lyricsString = await SearchAsync(artist, title);
        }
        catch (Exception ex)
        {
            return new LyricsResult();
        }

        if (string.IsNullOrWhiteSpace(lyricsString))
        {
            return new LyricsResult();
        }

        var lyrics = VictoriaLyrics.Parse(Provider, track, lyricsString);

        return new LyricsResult(lyrics);
    }

    protected abstract ValueTask<string> SearchAsync(string artist, string title);

    protected abstract IProvider Provider { get; }
    public abstract int Priority { get; }
}