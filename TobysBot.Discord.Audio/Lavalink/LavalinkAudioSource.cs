using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkAudioSource : IAudioSource
    {
        private readonly LavaNode<XLavaPlayer> _node;

        public LavalinkAudioSource(LavaNode<XLavaPlayer> node)
        {
            _node = node;
        }
        
        public async Task<IPlayable> SearchAsync(string query)
        {
            var result = Uri.IsWellFormedUriString(query, UriKind.Absolute)
                ? await _node.SearchAsync(SearchType.Direct, query)
                : await _node.SearchYouTubeAsync(query);

            return result.Status switch
            {
                SearchStatus.SearchResult => new LavalinkTrack(result.Tracks.FirstOrDefault()),
                SearchStatus.TrackLoaded => new LavalinkTrack(result.Tracks.FirstOrDefault()),
                SearchStatus.PlaylistLoaded => new LavalinkPlaylist(result.Tracks.Take(50), query, result.Playlist.Name,
                    result.Playlist.SelectedTrack),
                SearchStatus.NoMatches => null,
                SearchStatus.LoadFailed => throw new Exception(result.Exception.Message),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}