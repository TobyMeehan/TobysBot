using Discord;
using YoutubeExplode.Videos;

namespace TobysBot.Music.Search;

public class YouTubePlaylist : IPlaylist
{
    public YouTubePlaylist(YoutubeExplode.Playlists.IPlaylist playlist, IEnumerable<IVideo> videos, IUser requestedBy)
    {
        Title = playlist.Title;
        Url = playlist.Url;
        Tracks = from video in videos select new YouTubeTrack(video, requestedBy);
    }
    
    public string Title { get; }
    public string Url { get; }
    public IEnumerable<ITrack> Tracks { get; }
}