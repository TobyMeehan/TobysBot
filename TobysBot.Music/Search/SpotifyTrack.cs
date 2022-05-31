using SpotifyAPI.Web;

namespace TobysBot.Music.Search;

public class SpotifyTrack : ITrack
{
    public SpotifyTrack(FullTrack track, string audioUrl, TimeSpan duration)
    {
        Title = track.Name;
        Url = $"https://open.spotify.com/track/{track.Id}";
        AudioUrl = audioUrl;
        Duration = duration;
    }
    
    public string Title { get; }
    public string Url { get; }
    public string AudioUrl { get; }
    public TimeSpan Duration { get; }
}