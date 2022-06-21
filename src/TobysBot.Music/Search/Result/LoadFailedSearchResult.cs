namespace TobysBot.Music.Search.Result;

public class LoadFailedSearchResult : ISearchResult
{
    public LoadFailedSearchResult()
    {
        Reason = "Load failed.";
    }
    
    public LoadFailedSearchResult(string reason)
    {
        Reason = reason;
    }

    public LoadFailedSearchResult(string reason, Exception exception)
    {
        Reason = reason;
        Exception = exception;
    }
    
    public string Reason { get; }

    public Exception? Exception { get; }
}