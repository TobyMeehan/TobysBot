using Discord;

namespace TobysBot.Music;

/// <summary>
/// Resolver to load a search url.
/// </summary>
public interface ISearchResolver
{
    /// <summary>
    /// Whether the resolver can parse the specified uri.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    bool CanResolve(Uri uri);

    /// <summary>
    /// Attempts to resolve the specified uri.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="requestedBy"></param>
    /// <returns></returns>
    Task<ISearchResult> ResolveAsync(Uri uri, IUser requestedBy);
    
    /// <summary>
    /// Priority of the resolver.
    /// </summary>
    int Priority { get; }
}