namespace TobysBot.Voice.Effects;

public class BassBoostEffect : BaseEffect
{
    public BassBoostEffect(double percent)
    {
        Multiplier = percent / 100d;
    } 
    
    protected override double Multiplier { get; }

    protected override IEnumerable<EqualizerBand> DefaultEqualizer => new[]
    {
        0.2d,
        0.15d,
        0.1d,
        0.05d,
        0.0d,
        -0.05d,
        -0.1d,
        -0.1d,
        -0.1d,
        -0.1d,
        -0.1d,
        -0.1d,
        -0.1d,
        -0.1d,
        -0.1d
    }.Select(x => new EqualizerBand(x));
}