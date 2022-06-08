using TobysBot.Voice.Configuration;

namespace TobysBot.Music.Configuration;

public class MusicOptions
{
    public MusicEmbedOptions Embeds { get; set; }
    public LavalinkOptions Search { get; set; }
    public SpotifyOptions Spotify { get; set; }
}

public class MusicEmbedOptions
{
    public string NotFoundErrorDescription { get; set; }
    public string NotPlayingErrorDescription { get; set; }
    public string AlreadyPlayingErrorDescription { get; set; }
    public string AlreadyPausedErrorDescription { get; set; }
    public string CannotParseTimestampErrorDescription { get; set; }
    public string TimestampTooLongErrorDescription { get; set; }
    public string FastForwardTooFarErrorDescription { get; set; }
    public string RewindTooFarErrorDescription { get; set; }
    public string NoPreviousTrackErrorDescription { get; set; }
    public string PositionOutOfRangeErrorDescription { get; set; }
}

public class SpotifyOptions
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}