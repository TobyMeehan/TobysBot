using Discord;

namespace TobysBot.Music;

/// <summary>
/// Service to search for music based on a query.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Searches for the specified query.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="requestedBy"></param>
    /// <returns></returns>
    Task<ISearchResult> SearchAsync(string query, IUser requestedBy);

    /// <summary>
    /// Searches for music in the specified message attachments.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="requestedBy"></param>
    /// <returns></returns>
    Task<ISearchResult> LoadAttachmentsAsync(IMessage message, IUser requestedBy);
}