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
        SourceUrl = track.SourceUrl;
    }
    
    public string Url { get; }
    public string Title { get; }
    public string Id { get; }
    public TimeSpan Duration { get; }
    public string Author { get; }
    public string SourceUrl { get; }
}

public class MemoryActiveTrack : MemoryTrack, IActiveTrack
{
    public MemoryActiveTrack(ITrack track, TimeSpan position) : base(track)
    {
        Position = position;
    }

    public TimeSpan Position { get; }
}