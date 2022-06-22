namespace TobysBot.Voice.Effects;

public interface IPreset
{
    IEqualizer Equalizer { get; }
    
    double Speed { get; }
    double Pitch { get; }
    
    double Rotation { get; }
}