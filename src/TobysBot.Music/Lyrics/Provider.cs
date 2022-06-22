namespace TobysBot.Music.Lyrics;

public struct Provider : IProvider
{
    public Provider(string name, string url)
    {
        Name = name;
        Url = url;
    }

    public string Name { get; }
    public string Url { get; }
}