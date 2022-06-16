using TobysBot.Voice.Effects;
using EqualizerBand = Victoria.Filters.EqualizerBand;

namespace TobysBot.Voice.Extensions;

public static class EffectExtensions
{
    public static Victoria.Filters.EqualizerBand[] ToLavalinkEqualizer(this IEffect effect)
    {
        var i = 0;
        var result = new List<EqualizerBand>();

        foreach (var band in effect.Equalizer)
        {
            result.Add(new EqualizerBand(i, band.Gain));
            i++;
        }

        return result.ToArray();
    }
}