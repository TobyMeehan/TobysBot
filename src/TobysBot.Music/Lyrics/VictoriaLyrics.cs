namespace TobysBot.Music.Lyrics;

public struct VictoriaLyrics : ILyrics
{
    private VictoriaLyrics(IProvider provider, IEnumerable<ILine> lines, ITrack track)
    {
        Provider = provider;
        Lines = lines;
        Track = track;
    }

    public IProvider Provider { get; }
    public IEnumerable<ILine> Lines { get; }
    public ITrack Track { get; }

    public static VictoriaLyrics Parse(IProvider provider, ITrack track, string lyrics)
    {
        var lines = new List<ILine>();

        foreach (string line in lyrics.Split(Environment.NewLine))
        {
            if (line.Contains('['))
            {
                lines.Add(new Header(line.Replace("[", "").Replace("]", "")));
                continue;
            }
            
            lines.Add(new Line(line));
        }

        return new VictoriaLyrics(provider, lines, track);
    }
}