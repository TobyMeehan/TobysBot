using Discord;

namespace TobysBot.Music;

public interface ISearchService
{
    Task<ISearchResult> SearchAsync(string query, IUser requestedBy);

    Task<ISearchResult> LoadAttachmentsAsync(IMessage message, IUser requestedBy);
}