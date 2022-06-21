using System.Diagnostics.CodeAnalysis;

namespace TobysBot.Music.Lyrics;

public interface ILyricsResult
{
    [MemberNotNullWhen(true, nameof(Lyrics))]
    bool Success { get; }
    
    ILyrics? Lyrics { get; }
}