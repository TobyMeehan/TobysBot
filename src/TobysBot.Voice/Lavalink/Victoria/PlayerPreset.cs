using TobysBot.Voice.Effects;

namespace TobysBot.Voice.Lavalink.Victoria;

public class PlayerPreset : IPreset
{
    public PlayerPreset()
    {
        
    }

    public PlayerPreset(IPreset preset)
    {
        Speed = preset.Speed;
        Pitch = preset.Pitch;
        Rotation = preset.Rotation;
        Equalizer = new PlayerEqualizer(preset.Equalizer);
    }
    
    public PlayerEqualizer Equalizer { get; set; } = new();
    public double Speed { get; set; } = 1;
    public double Pitch { get; set; } = 1;
    public double Rotation { get; set; }

    IEqualizer IPreset.Equalizer => Equalizer;
}