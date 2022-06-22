namespace TobysBot.Music.Search.Result;

public class TrackResult : ISearchResult
{
    public TrackResult(ITrack track)
    {
        Track = track;
    }

    public ITrack Track { get; }
}