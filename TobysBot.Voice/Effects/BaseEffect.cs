namespace TobysBot.Voice.Effects;

public abstract class BaseEffect : IEffect
{
    protected abstract double Multiplier { get; }
    protected abstract IEnumerable<EqualizerBand> DefaultEqualizer { get; }

    public IEnumerable<EqualizerBand> Equalizer => DefaultEqualizer.Select(x =>
        new EqualizerBand(Math.Clamp(x.Gain * Multiplier, -0.25, 1)));
    
    public ushort Volume { get; }
}