namespace TobysBot.Music.Lyrics;

public struct VictoriaLyrics : ILyrics
{
    public VictoriaLyrics(IProvider provider, IEnumerable<ILine> lines)
    {
        Provider = provider;
        Lines = lines;
    }

    public IProvider Provider { get; }
    public IEnumerable<ILine> Lines { get; }

    public static VictoriaLyrics Parse(IProvider provider, string title, string lyrics)
    {
        var lines = new List<ILine> { new Title(title) };

        foreach (var line in lyrics.Split("\n"))
        {
            if (line.Contains('['))
            {
                lines.Add(new Header(line.TrimStart('[').TrimEnd(']')));
                continue;
            }
            
            lines.Add(new Line(line));
        }

        return new VictoriaLyrics(provider, lines);
    }
}