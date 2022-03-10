using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Extensions;

public static class LavaNodeExtensions
{
    public static async Task<LavaTrack> LoadTrackAsync(this LavaNode node, ITrack track)
    {
        var search = await node.SearchAsync(SearchType.Direct, track.Url);

        if (search.Status != SearchStatus.TrackLoaded)
        {
            throw new Exception(search.Exception.Message);
        }

        return search.Tracks.First();
    }
}