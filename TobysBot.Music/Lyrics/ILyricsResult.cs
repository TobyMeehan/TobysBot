namespace TobysBot.Music.Lyrics;

public interface ILyricsResult
{
    bool Success { get; }
    ILyrics Lyrics { get; }
}