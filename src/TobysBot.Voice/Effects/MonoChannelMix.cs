namespace TobysBot.Voice.Effects;

public class MonoChannelMix : IChannelMix
{
    public double LeftToRight => 0.5;
    public double LeftToLeft => 0.5;
    public double RightToLeft => 0.5;
    public double RightToRight => 0.5;
}