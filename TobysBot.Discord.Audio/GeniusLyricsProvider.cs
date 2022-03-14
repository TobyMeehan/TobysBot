using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TobysBot.Discord.Audio.Extensions;
using TobysBot.Discord.Audio.Lavalink;
using Victoria;
using Victoria.Resolvers;

namespace TobysBot.Discord.Audio;


// Mostly ripped from https://github.com/Yucked/Victoria/blob/v5/src/Resolvers/LyricsResolver.cs
// probably not a good idea but better than querying lavalink just to get lyrics
public class GeniusLyricsProvider : ILyricsProvider
{
    
    private readonly Regex _paranReg
        = new Regex(@"(\(.*?\))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Regex _artistReg
        = new Regex(@"\w+.\w+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private (string artist, string title) GetArtistAndTitle(ITrack track)
    {
        var title = _paranReg.Replace(track.Title, string.Empty);
        title = title.Replace("&", "and");
        var titleSplit = title.Split('-');

        var artistSplit = track.Author.Split('-');

        if (titleSplit.Length == 1 && artistSplit.Length > 1) {
            return (artistSplit[0].Trim(), title.Trim());
        }

        var artist = _artistReg.Match(titleSplit[0]).Value;
        if (artist.Equals(titleSplit[0], StringComparison.OrdinalIgnoreCase) ||
            artist.Equals(track.Author, StringComparison.OrdinalIgnoreCase)) {
            return (titleSplit[0].Trim(), titleSplit[1].Trim());
        }
        
        return (artist, titleSplit[1].Trim());
    }
    
    public async Task<string> GetLyricsAsync(ITrack track)
    {
        var (artist, title) = GetArtistAndTitle(track);
        
        return await LyricsResolver.SearchGeniusAsync(artist, title);
    }
}