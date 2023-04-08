using Discord;
using SpotifyAPI.Web;

namespace TobysBot.Music.Spotify;

public class SpotifyTrack : ITrack
{
    public SpotifyTrack(FullTrack track, IUser requestedBy)
    {
        Id = track.Id;
        Title = track.Name;
        Author = track.Artists.First().Name;
        Duration = TimeSpan.FromMilliseconds(track.DurationMs);
        RequestedBy = requestedBy;
    }

    public SpotifyTrack(SimpleTrack track, IUser requestedBy)
    {
        Id = track.Id;
        Title = track.Name;
        Author = track.Artists.First().Name;
        Duration = TimeSpan.FromMilliseconds(track.DurationMs);
        RequestedBy = requestedBy;
    }

    public string Id { get; }
    public string Title { get; }
    public string Author { get; }
    public string Url => $"https://open.spotify.com/track/{Id}";
    public TimeSpan Duration { get; }
    public IUser RequestedBy { get; }
}