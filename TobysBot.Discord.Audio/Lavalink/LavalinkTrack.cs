using System;
using System.Collections.Generic;
using System.Text;
using Victoria;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkTrack : ITrack
    {
        public LavalinkTrack(LavaTrack track, string title, string author)
        {
            Track = track;
            Title = title;
            Author = author;
        }

        public string Id => Track.Id;

        public string Author { get; }

        public string Title { get; }

        public TimeSpan Duration => Track.Duration;

        public TimeSpan Position => Track.Position;

        public string Url => SourceUrl;

        public string SourceUrl => Track.Url;

        public LavaTrack Track { get; }
    }
    
    public class LavalinkActiveTrack : LavalinkTrack, IActiveTrack
    {
        public LavalinkActiveTrack(LavaTrack track, string title, string author) : base (track, title, author)
        {
        }
    }
}
