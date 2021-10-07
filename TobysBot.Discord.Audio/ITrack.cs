using System;
using System.Collections.Generic;
using System.Text;

namespace TobysBot.Discord.Audio
{
    public interface ITrack
    {
        string Id { get; }
        string Author { get; }
        string Title { get; }
        TimeSpan Duration { get; }
        string Url { get; }
    }
}
