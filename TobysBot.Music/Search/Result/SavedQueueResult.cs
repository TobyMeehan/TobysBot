namespace TobysBot.Music.Search.Result;

public class SavedQueueResult : ISearchResult
{
    public ISavedQueue Queue { get; }

    public SavedQueueResult(ISavedQueue queue)
    {
        Queue = queue;
    }
}