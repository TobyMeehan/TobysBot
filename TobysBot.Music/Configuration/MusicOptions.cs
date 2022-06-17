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
    public string NotFoundErrorDescription { get; set; } = "No results for that query.";
    public string NotPlayingErrorDescription { get; set; } = "I'm not playing anything.";
    public string AlreadyPlayingErrorDescription { get; set; } = "The track is already playing!";
    public string AlreadyPausedErrorDescription { get; set; } = "The track is already paused!";
    public string CannotParseTimestampErrorDescription { get; set; } = "Could not parse that timestamp.";
    public string TimestampTooLongErrorDescription { get; set; } = "The track is not that long.";
    public string FastForwardTooFarErrorDescription { get; set; } = "Can't fast forward beyond the track!";
    public string RewindTooFarErrorDescription { get; set; } = "Can't rewind to before the track!";
    public string NoPreviousTrackErrorDescription { get; set; } = "No previous track to jump to.";
    public string PositionOutOfRangeErrorDescription { get; set; } = "No track at that position";
}

public class SpotifyOptions
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}