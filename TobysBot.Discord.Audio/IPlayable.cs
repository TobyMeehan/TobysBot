using System;
using System.Collections.Generic;
using System.Text;

namespace TobysBot.Discord.Audio
{
    public interface IPlayable
    {
        string Url { get; }
        
        string SourceUrl { get; }
        
        string Title { get; }
    }
}
