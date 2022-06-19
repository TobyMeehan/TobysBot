namespace TobysBot.Music;

public interface ISearchResolver
{
    bool CanResolve(Uri uri);

    Task<ISearchResult> ResolveAsync(Uri uri);
    
    int Priority { get; }
}