namespace TobysBot.Voice.Effects;

public interface IEffect
{
    IEnumerable<EqualizerBand> Equalizer { get; }
    
    ushort Volume { get; }
}