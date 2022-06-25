namespace TobysBot.Voice.Effects;

public interface IChannelMix
{
    double LeftToRight { get; }
    double LeftToLeft { get; }
    double RightToLeft { get; }
    public double RightToRight { get; }
}