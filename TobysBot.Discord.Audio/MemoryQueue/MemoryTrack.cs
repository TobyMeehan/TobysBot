using System;

namespace TobysBot.Discord.Audio.MemoryQueue;

public class MemoryTrack : ITrack
{
    public MemoryTrack(ITrack track)
    {
        Url = track.Url;
        Title = track.Title;
        Id = track.Id;
        Duration = track.Duration;
        Author = track.Author;
    }
    
    public string Url { get; }
    public string Title { get; }
    public string Id { get; }
    public TimeSpan Duration { get; }
    public string Author { get; }
}