using TobysBot.Voice;
using YoutubeExplode;

namespace TobysBot.Music.Spotify;

public class SpotifyAudioResolver : ISoundResolver
{
    private readonly YoutubeClient _youtube;

    public SpotifyAudioResolver(YoutubeClient youtube)
    {
        _youtube = youtube;
    }
    
    public bool CanResolve(ITrack track)
    {
        return track is SpotifyTrack;
    }

    public async Task<ISound> ResolveAsync(ITrack track)
    {
        var search = _youtube.Search.GetVideosAsync($"{track.Author} {track.Title}");

        await foreach (var video in search)
        {
            if (!video.Duration.HasValue)
            {
                continue;
            }

            if (video.Duration.Value - track.Duration > TimeSpan.FromSeconds(2))
            {
                continue;
            }

            return new Sound(track.Title, video.Url, video.Duration.Value);
        }

        throw new Exception("Unable to load track audio.");
    }

    public int Priority => 100;
}