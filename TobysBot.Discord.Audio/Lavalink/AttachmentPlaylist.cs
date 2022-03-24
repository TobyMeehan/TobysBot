using System.Collections;
using System.Collections.Generic;
using Discord;
using Victoria;

namespace TobysBot.Discord.Audio.Lavalink;

public class AttachmentPlaylist : IPlaylist
{
    private readonly List<ITrack> _tracks = new List<ITrack>();

    public AttachmentPlaylist(IMessage message)
    {
        Url = message.GetJumpUrl();
        Title = "Attachments";
    }
    
    public string Url { get; }
    public string Title { get; }

    public void Add(ITrack track)
    {
        _tracks.Add(track);
    }
    
    public IEnumerator<ITrack> GetEnumerator()
    {
        return _tracks.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}