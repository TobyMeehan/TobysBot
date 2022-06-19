using System.Collections;

namespace TobysBot.Voice.Effects;

public class Equalizer : BaseEqualizer
{
    public Equalizer()
    {
        Bands = base.Bands;
    }

    public Equalizer(IEnumerable<Band> bands)
    {
        Bands = bands;
    }

    protected override IEnumerable<Band> Bands { get; }
}