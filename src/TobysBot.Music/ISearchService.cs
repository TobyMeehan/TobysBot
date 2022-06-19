using Discord;

namespace TobysBot.Music;

public interface ISearchService
{
    Task<ISearchResult> SearchAsync(string query);

    Task<ISearchResult> LoadAttachmentsAsync(IMessage message);
}