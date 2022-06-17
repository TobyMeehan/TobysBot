using Victoria;
using Victoria.Responses.Search;

namespace TobysBot.Voice.Extensions;

public static class LavaNodeExtensions
{
    public static async Task<LavaTrack> LoadSoundAsync<T>(this LavaNode<T> lavaNode, ISound sound) where T : LavaPlayer
    {
        var result = await lavaNode.SearchAsync(SearchType.Direct, sound.Url);

        if (result.Status is not SearchStatus.TrackLoaded)
        {
            throw new Exception(result.Exception.Message);
        }

        return result.Tracks.First();
    }
}