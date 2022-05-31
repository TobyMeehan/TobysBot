namespace TobysBot.Music.Exceptions;

public class NoResultsException : Exception
{
    public NoResultsException()
    {
        
    }

    public NoResultsException(string query) : base($"No results for {query}")
    {
        
    }
}