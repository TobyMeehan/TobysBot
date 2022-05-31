namespace TobysBot.Music.Search.Result;

public class PlaylistResult : ISearchResult
{
    public PlaylistResult(IPlaylist playlist)
    {
        Playlist = playlist;
    }

    public IPlaylist Playlist { get; }
}