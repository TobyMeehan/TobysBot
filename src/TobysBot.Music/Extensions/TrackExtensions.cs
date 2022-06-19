using TobysBot.Music.Voice;
using TobysBot.Voice;

namespace TobysBot.Music.Extensions;

public static class TrackExtensions
{
    public static ISound ToSound(this ITrack track)
    {
        return new TrackSound(track);
    }
}