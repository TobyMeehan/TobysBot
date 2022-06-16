namespace TobysBot.Voice.Effects;

public struct EqualizerBand
{
    public EqualizerBand(double gain)
    {
        Gain = gain;
    }

    public double Gain { get; set; }
}