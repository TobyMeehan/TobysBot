using Discord;

namespace TobysBot.Music.Data;

public class SavedTrack : ITrack
{
    public SavedTrack()
    {
        
    }

    public SavedTrack(ITrack track)
    {
        Title = track.Title;
        Author = track.Author;
        Url = track.Url;
        Duration = track.Duration;
    }
    
    // -- Data to write
    
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Url { get; set; } = null!;
    public TimeSpan Duration { get; set; }

    // -- --

    public IUser RequestedBy { get; set; } = null!;
}