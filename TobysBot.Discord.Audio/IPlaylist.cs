using System;
using System.Collections.Generic;
using System.Text;

namespace TobysBot.Discord.Audio
{
    public interface IPlaylist : IPlayable, IEnumerable<ITrack>
    {

    }
}
