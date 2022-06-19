using Discord;
using TobysBot.Data;

namespace TobysBot.Music;

public interface ISavedQueue
{
    string Name { get; }
    ulong UserId { get; }
    IEnumerable<ISavedTrack> Tracks { get; }
}