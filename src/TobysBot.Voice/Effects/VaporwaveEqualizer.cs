namespace TobysBot.Voice.Effects;

public class VaporwaveEqualizer : BaseEqualizer
{
    protected override IEnumerable<double> Gain => new[]
    {
        0.13d,
        0.15d,
        0.13d,
        0.07d,
        0.0d,
        -0.05d,
        -0.05d,
        -0.05d,
        -0.05d,
        0.0d,
        0.05d,
        0.1d,
        0.15d,
        0.2d,
        0.2d
    };
}