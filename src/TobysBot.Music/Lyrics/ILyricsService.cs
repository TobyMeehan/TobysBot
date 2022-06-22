namespace TobysBot.Music.Lyrics;

public interface ILyricsService
{
    Task<ILyricsResult> GetLyricsAsync(ITrack track);
}