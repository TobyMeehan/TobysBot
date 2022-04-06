using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Extensions;

public static class SearchResponseExtensions
{
    public static bool IsTrackLoadedStatus(this SearchResponse searchResponse)
    {
        return searchResponse.Status is SearchStatus.TrackLoaded;
    }

    public static bool IsPlaylistLoadedStatus(this SearchResponse searchResponse)
    {
        return searchResponse.Status is SearchStatus.PlaylistLoaded;
    }

    public static bool IsTrackOrPlaylistLoadedStatus(this SearchResponse searchResponse)
    {
        return searchResponse.Status is SearchStatus.TrackLoaded or SearchStatus.PlaylistLoaded;
    }
    
    public static bool IsSearchResultStatus(this SearchResponse searchResponse)
    {
        return searchResponse.Status is SearchStatus.SearchResult or SearchStatus.NoMatches;
    }
}