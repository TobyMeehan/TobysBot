using System.Text.RegularExpressions;
using TobysBot.Music.Spotify;

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

        var punctuationReg
            = new Regex(@"[^0-9A-Za-z._\-\s]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var artistReg
            = new Regex(@"\w+.\w+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var remasteredReg
            = new Regex(@"- ([0-9]{4} )?Remaster(ed)?( [0-9]{4})?");

        string title = parenReg.Replace(track.Title, string.Empty);
        title = punctuationReg.Replace(title, string.Empty);
        title = remasteredReg.Replace(title, string.Empty);
        title = title.Replace("&", "and");
        
        string[] titleSplit = title.Split('-');
        string[] artistSplit = track.Author.Split('-');
        
        if (track is SpotifyTrack spotifyTrack)
        {
            return (spotifyTrack.Author, titleSplit[0].Trim());
        }
        
        if (titleSplit.Length == 1 && artistSplit.Any())
        {
            return (artistSplit[0].Trim(), title.Trim());
        }

        if (titleSplit[0].Equals(artistSplit[0], StringComparison.OrdinalIgnoreCase))
        {
            return (artistSplit[0].Trim(), titleSplit[1].Trim());
        }

        if (titleSplit[1].Equals(artistSplit[0], StringComparison.OrdinalIgnoreCase))
        {
            return (artistSplit[0].Trim(), titleSplit[0].Trim());
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
        catch (Exception)
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