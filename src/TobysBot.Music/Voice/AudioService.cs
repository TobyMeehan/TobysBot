using System.Text;
using FastHashes;
using Microsoft.Extensions.Caching.Memory;
using TobysBot.Voice;

namespace TobysBot.Music.Voice;

public class AudioService : IAudioService
{
    private readonly IEnumerable<ISoundResolver> _resolvers;
    private readonly IMemoryCache _cache;

    public AudioService(IEnumerable<ISoundResolver> resolvers, IMemoryCache cache)
    {
        _resolvers = resolvers;
        _cache = cache;
    }
    
    public async Task<ISound> LoadAudioAsync(ITrack track)
    {
        return await _cache.GetOrCreateAsync(GetTrackHash(track), _ => ResolveAsync(track));
    }
    
    private async Task<ISound> ResolveAsync(ITrack track)
    {
        foreach (var resolver in _resolvers)
        {
            if (resolver.CanResolve(track))
            {
                return await resolver.ResolveAsync(track);
            }
        }

        return new TrackSound(track);
    }
    
    private string GetTrackHash(ITrack track)
    {
        var hash = new FarmHash32().ComputeHash(Encoding.Unicode.GetBytes(track.Url));
        return Convert.ToBase64String(hash);
    }
}