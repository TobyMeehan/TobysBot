namespace TobysBot.Music.Lyrics;

public interface IProvider
{
    string Name { get; }
    string Url { get; }
}