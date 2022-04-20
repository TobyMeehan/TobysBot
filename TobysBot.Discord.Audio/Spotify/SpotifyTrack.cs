using System;
using SpotifyAPI.Web;

namespace TobysBot.Discord.Audio.Spotify;

public class SpotifyTrack : ITrack
{
    private readonly ITrack _innerTrack;

    public SpotifyTrack(FullTrack track, ITrack innerTrack)
    {
        _innerTrack = innerTrack;
        Url = track.Href;
        Title = track.Name;
        Author = track.Artists[0].Name;
    }

    public string Url { get; }
    public string Title { get; }
    public string Author { get; }
    
    public string Id => _innerTrack.Id;
    public TimeSpan Duration => _innerTrack.Duration;
    public string SourceUrl => _innerTrack.SourceUrl;
}