using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Discord;
using SpotifyAPI.Web;
using TobysBot.Discord.Audio.Extensions;
using TobysBot.Discord.Audio.Spotify;
using Victoria;
using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkAudioSource : IAudioSource
    {
        private readonly LavaNode<XLavaPlayer> _node;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISpotifyClient _spotify;

        public LavalinkAudioSource(LavaNode<XLavaPlayer> node, IHttpClientFactory httpClientFactory, ISpotifyClient spotify)
        {
            _node = node;
            _httpClientFactory = httpClientFactory;
            _spotify = spotify;
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
                    
                    case "open.spotify.com":

                        return await LoadFromSpotifyAsync(uri);
                    
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
                SearchStatus.LoadFailed => new NotPlayable(new Exception(result.Exception.Message)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private async Task<IPlayable> LoadFromSpotifyAsync(Uri uri)
        {
            return uri.Segments[1] switch
            {
                "track/" => await LoadSpotifyTrackAsync(uri.Segments[2]),
                "playlist/" => await LoadSpotifyPlaylistAsync(uri.Segments[2]),
                _ => new NotPlayable(new Exception("Invalid spotify url."))
            };
        }

        private async Task<IPlayable> LoadSpotifyTrackAsync(string id)
        {
            var track = await _spotify.Tracks.Get(id);
            
            var query = $"{track.Artists[0].Name} {track.Name}";
            
            var search = await _node.SearchYouTubeAsync(query);

            if (search.Status is not SearchStatus.SearchResult)
            {
                return new NotPlayable(new Exception("No results for spotify track."));
            }

            foreach (var ytTrack in search.Tracks)
            {
                var uri = new Uri(ytTrack.Url);
                var playable = await LoadFromYoutubeAsync(new Uri($"{uri.Scheme}://music.youtube.com{uri.PathAndQuery}"));

                if (playable is not ITrack result)
                {
                    continue;
                }

                return new SpotifyTrack(track, result);
            }

            return new NotPlayable(new Exception("Could not find source for spotify track."));
        }

        private async Task<IPlayable> LoadSpotifyPlaylistAsync(string id)
        {
            var playlist = await _spotify.Playlists.Get(id);

            if (playlist.Tracks?.Items is null)
            {
                return new NotPlayable(new Exception("Could not load spotify playlist."));
            }
            
            var tracks = new List<SpotifyTrack>();

            foreach (var track in playlist.Tracks.Items)
            {
                if (track.Track.Type is ItemType.Episode)
                {
                    return new NotPlayable(new Exception("Podcasts are not currently supported."));
                }

                var playable = await LoadSpotifyTrackAsync((track.Track as FullTrack)?.Id);

                if (playable is not SpotifyTrack spotifyTrack)
                {
                    continue;
                }
                
                tracks.Add(spotifyTrack);
            }

            if (!tracks.Any())
            {
                return new NotPlayable(new Exception("Could not load spotify playlist."));
            }

            return new SpotifyPlaylist(playlist, tracks);
        }

        private async Task<IPlayable> LoadFromYoutubeAsync(Uri uri)
        {
            var query = HttpUtility.ParseQueryString(uri.Query);

            return uri.Segments[1] switch
            {
                "watch" => await LoadYoutubeTrackAsync(query["v"]),
                "playlist" => await LoadYoutubePlaylistAsync(query["list"]),
                "shorts/" => await LoadYoutubeTrackAsync(uri.Segments.Last()),
                _ => new NotPlayable(new Exception("Could not parse YouTube url."))
            };
        }

        private async Task<IPlayable> LoadYoutubeTrackAsync(string id)
        {
            var url = $"https://youtu.be/{id}";

            var result = await _node.SearchAsync(SearchType.Direct, url);

            if (!result.IsTrackLoadedStatus())
            {
                return new NotPlayable(new Exception(result.Exception.Message));
            }

            return new LavalinkTrack(result.Tracks.First());
        }

        private async Task<IPlayable> LoadYoutubePlaylistAsync(string id, int index = 0)
        {
            var url = $"https://youtube.com/playlist?list={id}";
            
            var result = await _node.SearchAsync(SearchType.Direct, url);

            if (!result.IsPlaylistLoadedStatus())
            {
                return new NotPlayable(new Exception(result.Exception.Message));
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

            return new NotPlayable(new Exception(result.Exception.Message));
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