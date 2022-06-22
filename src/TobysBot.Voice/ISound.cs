namespace TobysBot.Voice;

public interface ISound
{
    string Title { get; }
    
    string Url { get; }
    
    TimeSpan Duration { get; }
}