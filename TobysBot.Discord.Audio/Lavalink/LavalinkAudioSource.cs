using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkAudioSource : IAudioSource
    {
        private readonly LavaNode _node;

        public LavalinkAudioSource(LavaNode node)
        {
            _node = node;
        }
        
        public async Task<IPlayable> SearchAsync(string query)
        {
            var result = Uri.IsWellFormedUriString(query, UriKind.Absolute)
                ? await _node.SearchAsync(SearchType.Direct, query)
                : await _node.SearchYouTubeAsync(query);

            switch (result.Status)
            {
                case SearchStatus.SearchResult:
                    return new LavalinkTrack(result.Tracks.FirstOrDefault());
                
                case SearchStatus.TrackLoaded:
                    return new LavalinkTrack(result.Tracks.FirstOrDefault());
                
                case SearchStatus.PlaylistLoaded:
                    return new LavalinkPlaylist(result.Tracks, query, result.Playlist.Name, result.Playlist.SelectedTrack);
                
                default:
                    return null;
            }
        }
    }
}