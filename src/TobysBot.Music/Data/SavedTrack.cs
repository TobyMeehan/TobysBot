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
        AudioUrl = track.AudioUrl;
        Duration = track.Duration;
    }
    
    // -- Data to write
    
    public string Title { get; set; }
    public string Author { get; set; }
    public string Url { get; set; }
    public string AudioUrl { get; set; }
    public TimeSpan Duration { get; set; }
    
    // -- --
}