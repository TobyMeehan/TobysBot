namespace TobysBot.Voice.Effects;

public class ChannelMix : IChannelMix
{
    public double LeftToRight => 0;
    public double LeftToLeft => 1;
    public double RightToLeft => 0;
    public double RightToRight => 1;
}