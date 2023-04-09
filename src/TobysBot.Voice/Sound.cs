namespace TobysBot.Voice;

public class Sound : ISound
{
    public Sound(string title, string url, TimeSpan duration)
    {
        Title = title;
        Url = url;
        Duration = duration;
    }

    public string Title { get; }
    public string Url { get; }
    public TimeSpan Duration { get; }
}