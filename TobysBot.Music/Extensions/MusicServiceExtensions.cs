using Discord;

namespace TobysBot.Music.Extensions;

public static class MusicServiceExtensions
{
    public static async Task<IActiveTrack> GetTrackAsync(this IMusicService music, IGuild guild)
    {
        var queue = await music.GetQueueAsync(guild);

        return queue.CurrentTrack;
    }
}