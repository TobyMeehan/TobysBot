using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkAudioNode : IAudioNode
    {
        private readonly LavaNode _node;
        private readonly IQueue _queue;

        public LavalinkAudioNode(LavaNode node, IQueue queue)
        {
            _node = node;
            _queue = queue;
        }

        private LavaPlayer ThrowIfNoPlayer(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                throw new Exception("No player is connected to the guild.");
            }

            return player;
        }

        private async Task<LavaTrack> LoadTrackAsync(ITrack track)
        {
            SearchResponse search = await _node.SearchAsync(SearchType.Direct, track.Url);

            if (search.Status != SearchStatus.TrackLoaded)
            {
                throw new Exception("Error loading track.");
            }

            return search.Tracks.First();
        }

        public async Task JoinAsync(IVoiceChannel channel, ITextChannel textChannel = null)
        {
            if (_node.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                if (player.VoiceChannel != channel)
                {
                    await _node.LeaveAsync(player.VoiceChannel);

                    textChannel ??= player.TextChannel;
                }
            }

            await _node.JoinAsync(channel, textChannel);
        }

        public async Task LeaveAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                return;
            }

            await _node.LeaveAsync(player.VoiceChannel);
        }


        public async Task<ITrack> EnqueueAsync(IPlayable source, IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            SearchResponse search = await _node.SearchAsync(SearchType.Direct, source.Url);

            List<ITrack> tracks = new List<ITrack>();

            if (source is ITrack track)
            {
                if (search.Status != SearchStatus.TrackLoaded)
                {
                    throw new Exception("Error loading track.");
                }

                tracks.Add(track);
            }

            if (source is IPlaylist playlist)
            {
                if (search.Status != SearchStatus.PlaylistLoaded)
                {
                    throw new Exception("Error loading playlist.");
                }

                tracks.AddRange(playlist);
            }

            if (!player.HasTrack())
            {
                await player.PlayAsync(search.Tracks.First());

                tracks.RemoveAt(0);
            }

            if (tracks.Any())
            {
                await _queue.EnqueueAsync(guild, tracks);
            }

            return new LavalinkTrack(player.Track);
        }

        public async Task PauseAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await player.PauseAsync();
        }

        public async Task ResumeAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await player.ResumeAsync();
        }

        public async Task<ITrack> SkipAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            ITrack nextTrack = await _queue.DequeueAsync(guild);

            await player.PlayAsync(await LoadTrackAsync(nextTrack));

            return nextTrack;
        }

        public async Task ClearAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await _queue.ClearAsync(guild);

            await player.StopAsync();
        }

        public async Task StopAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await player.StopAsync();
        }


        public Task<IVoiceChannel> GetCurrentChannelAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                return Task.FromResult<IVoiceChannel>(null);
            }

            return Task.FromResult(player.VoiceChannel);
        }

        public Task<ITrack> GetCurrentTrackAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                return Task.FromResult<ITrack>(null);
            }

            if (!player.HasTrack())
            {
                return Task.FromResult<ITrack>(null);
            }

            return Task.FromResult<ITrack>(new LavalinkTrack(player.Track));
        }

        public async Task<IEnumerable<ITrack>> GetQueueAsync(IGuild guild)
        {
            return await _queue.GetAsync(guild);
        }
    }
}
