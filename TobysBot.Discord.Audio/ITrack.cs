using System;
using System.Collections.Generic;
using System.Text;

namespace TobysBot.Discord.Audio
{
    public interface ITrack : IPlayable
    {
        string Id { get; }
        TimeSpan Duration { get; }
        string Title { get; }
        string Author { get; }

    }
}
