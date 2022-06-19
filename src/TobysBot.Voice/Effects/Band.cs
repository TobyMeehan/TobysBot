namespace TobysBot.Voice.Effects;

public class Band
{
    public Band()
    {
        
    }
    
    public Band(double gain)
    {
        Gain = gain;
    }

    public double Gain { get; set; }
}