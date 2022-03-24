using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using TobysBot.Discord.Audio.Extensions;
using Victoria;
using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkAudioSource : IAudioSource
    {
        private readonly LavaNode<XLavaPlayer> _node;
        private readonly IHttpClientFactory _httpClientFactory;

        public LavalinkAudioSource(LavaNode<XLavaPlayer> node, IHttpClientFactory httpClientFactory)
        {
            _node = node;
            _httpClientFactory = httpClientFactory;
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

        public async Task<IPlayable> LoadAttachmentsAsync(IMessage message)
        {
            var playlist = new AttachmentPlaylist(message);
            
            foreach (var attachment in message.Attachments)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, attachment.Url);
            
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.SendAsync(request);

                var contentType = response.Content.Headers.ContentType?.MediaType;

                switch (contentType)
                {
                    case "audio/mpeg":
                    case "audio/wav":
                    case "audio/ogg":
                    case "video/mp4":
                    case "audio/m4a":
                    case "video/x-ms-wmv":
                    case "video/x-matroska":

                        var track = new LavalinkTrack(await _node.LoadTrackAsync(attachment.Url));
                        playlist.Add(track);
                        
                        break;
                }
            }

            if (!playlist.Any())
            {
                return null;
            }

            if (playlist.Count() == 1)
            {
                return playlist.First();
            }

            return playlist;
        }
    }
}