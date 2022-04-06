using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
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
            if (Uri.TryCreate(query, UriKind.Absolute, out var uri))
            {
                switch (uri.Host)
                {
                    case "youtube.com":
                    case "www.youtube.com":
                    case "music.youtube.com":
                        return await LoadFromYoutubeAsync(uri);
                    
                    default:
                        return await LoadFromUriAsync(uri);
                }
            }

            var result = await _node.SearchYouTubeAsync(query);

            return result.Status switch
            {
                SearchStatus.SearchResult => new LavalinkTrack(result.Tracks.FirstOrDefault()),
                SearchStatus.TrackLoaded => new LavalinkTrack(result.Tracks.FirstOrDefault()),
                SearchStatus.PlaylistLoaded => new LavalinkPlaylist(result.Tracks, query, result.Playlist.Name,
                    result.Playlist.SelectedTrack),
                SearchStatus.NoMatches => null,
                SearchStatus.LoadFailed => throw new Exception(result.Exception.Message),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private async Task<IPlayable> LoadFromYoutubeAsync(Uri uri)
        {
            var query = HttpUtility.ParseQueryString(uri.Query);

            return uri.Segments[1] switch
            {
                "watch" => await LoadYoutubeTrackAsync(query["v"]),
                "playlist" => await LoadYoutubePlaylistAsync(query["list"]),
                "shorts/" => await LoadYoutubeTrackAsync(uri.Segments.Last()),
                _ => throw new Exception("Could not parse YouTube url.")
            };
        }

        private async Task<IPlayable> LoadYoutubeTrackAsync(string id)
        {
            var url = $"https://youtu.be/{id}";

            var result = await _node.SearchAsync(SearchType.Direct, url);

            if (!result.IsTrackLoadedStatus())
            {
                throw new Exception(result.Exception.Message);
            }

            return new LavalinkTrack(result.Tracks.First());
        }

        private async Task<IPlayable> LoadYoutubePlaylistAsync(string id, int index = 0)
        {
            var url = $"https://youtube.com/playlist?list={id}";
            
            var result = await _node.SearchAsync(SearchType.Direct, url);

            if (!result.IsPlaylistLoadedStatus())
            {
                throw new Exception(result.Exception.Message);
            }

            return new LavalinkPlaylist(result.Tracks, url, result.Playlist.Name, index);
        }
        
        private async Task<IPlayable> LoadFromUriAsync(Uri uri)
        {
            var url = uri.AbsoluteUri;
            var result = await _node.SearchAsync(SearchType.Direct, url);

            if (result.IsTrackLoadedStatus())
            {
                return new LavalinkTrack(result.Tracks.First());
            }

            if (result.IsPlaylistLoadedStatus())
            {
                return new LavalinkPlaylist(result.Tracks, url, result.Playlist.Name, result.Playlist.SelectedTrack);
            }

            throw new Exception(result.Exception.Message);
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