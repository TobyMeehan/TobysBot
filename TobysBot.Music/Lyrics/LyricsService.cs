namespace TobysBot.Music.Lyrics;

public class LyricsService : ILyricsService
{
    private readonly IEnumerable<ILyricsResolver> _resolvers;

    public LyricsService(IEnumerable<ILyricsResolver> resolvers)
    {
        _resolvers = resolvers.OrderByDescending(x => x.Priority);
    }
    
    public async Task<ILyricsResult> GetLyricsAsync(ITrack track)
    {
        foreach (var resolver in _resolvers)
        {
            var result = await resolver.TryResolveAsync(track);

            if (result.Success)
            {
                return result;
            }
        }

        return new LyricsResult();
    }
}