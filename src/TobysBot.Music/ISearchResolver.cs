using Discord;

namespace TobysBot.Music;

public interface ISearchResolver
{
    bool CanResolve(Uri uri);

    Task<ISearchResult> ResolveAsync(Uri uri, IUser requestedBy);
    
    int Priority { get; }
}