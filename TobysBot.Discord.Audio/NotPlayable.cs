using System;

namespace TobysBot.Discord.Audio;

public class NotPlayable : IPlayable
{
    public NotPlayable(Exception exception)
    {
        Exception = exception;
    }

    public string Url => null;
    public string SourceUrl => null;
    public string Title => null;

    public Exception Exception { get; }
}