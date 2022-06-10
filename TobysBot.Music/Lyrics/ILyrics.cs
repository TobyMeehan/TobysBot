namespace TobysBot.Music.Lyrics;

public interface ILyrics
{
    IProvider Provider { get; }
    IEnumerable<ILine> Lines { get; }
}