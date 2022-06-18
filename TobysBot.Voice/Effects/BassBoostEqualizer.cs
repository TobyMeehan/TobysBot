namespace TobysBot.Voice.Effects;

public class BassBoostEqualizer : BaseEqualizer
{
    private readonly double _multiplier;

    public BassBoostEqualizer(double multiplier)
    {
        _multiplier = multiplier;
    }

    protected override IEnumerable<double> Gain => new[]
    {
        0.2d,
        0.15d,
        0.1d,
        0.05d,
        0.0d,
        -0.005d,
        -0.01d,
        -0.01d,
        -0.01d,
        -0.01d,
        -0.01d,
        -0.01d,
        -0.01d,
        -0.01d,
        -0.01d
    }.Select(x => x * _multiplier);
}