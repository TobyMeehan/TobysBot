using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Victoria;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkPlaylist : IPlaylist
    {
        private readonly IEnumerable<ITrack> _tracks;

        public LavalinkPlaylist(IEnumerable<LavaTrack> tracks, string url, string title, int startPos)
        {
            _tracks =
                from track in tracks.Skip(startPos)
                select new LavalinkTrack(track, track.Title, track.Author);
            
            Url = url;
            Title = title;
        }
        
        public string Url { get; }
        public string SourceUrl => Url;
        public string Title { get; }

        public IEnumerator<ITrack> GetEnumerator()
        {
            return _tracks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}