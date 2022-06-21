namespace TobysBot.Music.Lyrics;

public class LyricsResult : ILyricsResult
{
    public LyricsResult()
    {
        Success = false;
    }
    
    public LyricsResult(ILyrics lyrics)
    {
        Success = true;
        Lyrics = lyrics;
    }

    public bool Success { get; }
    public ILyrics? Lyrics { get; }
}