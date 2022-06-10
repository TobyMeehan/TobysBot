namespace TobysBot.Music.Lyrics;

public class Line : ILine
{
    public Line(string content)
    {
        Content = content;
    }

    public string Content { get; }
}

public class Title : Line
{
    public Title(string content) : base(content)
    {
    }
}

public class Header : Line
{
    public Header(string content) : base(content)
    {
    }
}