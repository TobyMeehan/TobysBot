using Discord;
using TobysBot.Data;

namespace TobysBot.Music;

public interface ISavedQueue : IEntity
{
    string Name { get; }
    ulong UserId { get; }
    IEnumerable<ITrack> Tracks { get; }
}