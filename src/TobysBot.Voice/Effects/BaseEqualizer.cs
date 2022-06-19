using System.Collections;

namespace TobysBot.Voice.Effects;

public abstract class BaseEqualizer : IEqualizer
{
    protected virtual IEnumerable<Band> Bands => Gain.Select(x => new Band(x));

    protected virtual IEnumerable<double> Gain => Enumerable.Repeat(0d, 15).ToArray();

    public IEnumerator<Band> GetEnumerator()
    {
        return Bands.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}