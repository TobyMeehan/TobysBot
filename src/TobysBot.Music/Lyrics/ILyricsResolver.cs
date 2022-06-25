namespace TobysBot.Music.Lyrics;

public interface ILyricsResolver
{
    Task<ILyricsResult> TryResolveAsync(ITrack track);
    
    int Priority { get; }
}