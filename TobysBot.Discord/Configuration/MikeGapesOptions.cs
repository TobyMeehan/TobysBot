using System.Collections.Generic;

namespace TobysBot.Discord.Configuration;

public class MikeGapesOptions
{
    public IEnumerable<MikeGapesServerOptions> Servers { get; set; }
}

public class MikeGapesServerOptions
{
    public ulong Id { get; set; }
    public ulong WordleChannel { get; set; }
    public ulong QuordleChannel { get; set; }
    public ulong OctordleChannel { get; set; }
    public ulong SedecordleChannel { get; set; }
}