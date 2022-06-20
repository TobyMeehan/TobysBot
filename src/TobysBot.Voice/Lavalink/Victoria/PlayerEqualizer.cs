using System.Collections;
using TobysBot.Voice.Effects;

namespace TobysBot.Voice.Lavalink;

public class PlayerEqualizer : IEqualizer
{
    public PlayerEqualizer()
    {
        
    }

    public PlayerEqualizer(IEqualizer equalizer)
    {
        _bands = equalizer.ToList();
    }
    
    private List<Band> _bands = new();

    public Band this[int band]
    {
        get
        {
            if (band is < 0 or > 14)
            {
                throw new IndexOutOfRangeException();
            }

            return _bands[band];
        }

        set
        {
            if (band is < 0 or > 14)
            {
                throw new IndexOutOfRangeException();
            }

            _bands[band] = value;
        }
    }
    
    public IEnumerator<Band> GetEnumerator()
    {
        return _bands.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}