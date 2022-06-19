using TobysBot.Data;

namespace TobysBot.Music.Data;

public class SavedTrack : ISavedTrack
{
    public SavedTrack()
    {
        
    }

    public SavedTrack(ITrack track)
    {
        Title = track.Title;
        Url = track.Url;
        Duration = track.Duration;
    }
    
    // -- Data to write
    
    public string Title { get; set; }
    public string Url { get; set; }
    public TimeSpan Duration { get; set; }
    
    // -- --
}