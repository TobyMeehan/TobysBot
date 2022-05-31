using YoutubeExplode.Videos;

namespace TobysBot.Music.Search;

public class YouTubePlaylist : IPlaylist, ISearchResult
{
    public YouTubePlaylist(YoutubeExplode.Playlists.IPlaylist playlist, IEnumerable<IVideo> videos)
    {
        Title = playlist.Title;
        Url = playlist.Url;
        Tracks = from video in videos select new YouTubeTrack(video);
    }
    
    public string Title { get; }
    public string Url { get; }
    public IEnumerable<ITrack> Tracks { get; }
}