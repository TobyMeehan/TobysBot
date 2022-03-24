using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Extensions;

public static class LavaNodeExtensions
{
    public static async Task<LavaTrack> LoadTrackAsync<T>(this LavaNode<T> node, ITrack track) where T : LavaPlayer
    {
        return await node.LoadTrackAsync(track.Url);
    }

    public static async Task<LavaTrack> LoadTrackAsync<T>(this LavaNode<T> node, string url) where T : LavaPlayer
    {
        var search = await node.SearchAsync(SearchType.Direct, url);

        if (search.Status != SearchStatus.TrackLoaded)
        {
            throw new Exception(search.Exception.Message);
        }

        return search.Tracks.First();
    }
}