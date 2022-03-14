using System;
using System.Collections.Generic;
using System.Text;
using Victoria;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkTrack : ITrack
    {
        public LavalinkTrack()
        {
            
        }

        public LavalinkTrack(LavaTrack track)
        {
            Track = track;
        }

        public string Id => Track.Id;

        public string Author => Track.Author;

        public string Title => Track.Title;

        public TimeSpan Duration => Track.Duration;

        public TimeSpan Position => Track.Position;

        public string Url => Track.Url;

        public LavaTrack Track { get; }
    }
    
    public class LavalinkActiveTrack : LavalinkTrack, IActiveTrack
    {
        public LavalinkActiveTrack(LavaTrack track) : base (track)
        {
        }
    }
}
