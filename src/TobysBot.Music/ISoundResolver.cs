using TobysBot.Voice;

namespace TobysBot.Music;

public interface ISoundResolver
{
    bool CanResolve(ITrack track);

    Task<ISound> ResolveAsync(ITrack track);
    
    int Priority { get; }
}