using Discord;
using YoutubeExplode.Videos;

namespace TobysBot.Music.YouTube;

public class YouTubePlaylist : IPlaylist
{
    public YouTubePlaylist(YoutubeExplode.Playlists.IPlaylist playlist, IEnumerable<IVideo> videos, IUser requestedBy)
    {
        Title = playlist.Title;
        Url = playlist.Url;
        Tracks = videos.Select(x => new YouTubeTrack(x, requestedBy));
    }
    
    public string Title { get; }
    public string Url { get; }
    public IEnumerable<ITrack> Tracks { get; }
}