using TobysBot.Voice;

namespace TobysBot.Music;

public interface IAudioService
{
    Task<ISound> LoadAudioAsync(ITrack track);
}