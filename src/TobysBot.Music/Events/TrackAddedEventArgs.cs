using Discord;

namespace TobysBot.Music.Events;

public class TrackAddedEventArgs
{
    public TrackAddedEventArgs(IEnumerable<ITrack> tracks)
    {
        Tracks = tracks;
    }

    public IEnumerable<ITrack> Tracks { get; }
}