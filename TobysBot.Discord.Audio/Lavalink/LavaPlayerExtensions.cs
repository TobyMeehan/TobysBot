using System;
using System.Collections.Generic;
using System.Text;
using Victoria;
using Victoria.Enums;

namespace TobysBot.Discord.Audio.Lavalink
{
    public static class LavaPlayerExtensions
    {
        public static bool IsPlaying(this LavaPlayer player)
        {
            return player.Track != null && (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused);
        }
    }
}
