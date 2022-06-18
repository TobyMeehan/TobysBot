using TobysBot.Voice.Effects;

namespace TobysBot.Voice.Lavalink;

public class PlayerPreset : IPreset
{
    public PlayerEqualizer Equalizer { get; set; } = new PlayerEqualizer();
    public double Speed { get; set; } = 1;
    public double Pitch { get; set; } = 1;
    public double Rotation { get; set; } = 0;

    IEqualizer IPreset.Equalizer => Equalizer;
}