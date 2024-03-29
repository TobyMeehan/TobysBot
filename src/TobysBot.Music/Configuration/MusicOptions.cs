﻿using TobysBot.Voice.Configuration;

namespace TobysBot.Music.Configuration;

public class MusicOptions
{
    public MusicEmbedOptions? Embeds { get; set; }
    public LavalinkOptions? Search { get; set; }
    public SpotifyOptions? Spotify { get; set; }
    public MusicDataOptions? Data { get; set; }
}

public class MusicEmbedOptions
{
    
}

public class SpotifyOptions
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}

public class MusicDataOptions
{
    public string? SavedQueueCollection { get; set; }
}