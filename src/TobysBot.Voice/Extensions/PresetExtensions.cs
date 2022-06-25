using TobysBot.Voice.Effects;
using Victoria.Filters;

namespace TobysBot.Voice.Extensions;

public static class PresetExtensions
{
    public static IEnumerable<IFilter> GetLavaFilters(this IPreset preset)
    {
        return new List<IFilter>
        {
            new TimescaleFilter { Speed = preset.Speed, Pitch = preset.Pitch, Rate = 1 },
            new RotationFilter { Hertz = preset.Rotation },
            new ChannelMixFilter
            {
                LeftToLeft = preset.ChannelMix.LeftToLeft, 
                LeftToRight = preset.ChannelMix.LeftToRight,
                RightToLeft = preset.ChannelMix.RightToLeft, 
                RightToRight = preset.ChannelMix.RightToRight
            }
        };
    }

    public static EqualizerBand[] GetLavaEqualizer(this IPreset preset)
    {
        int i = 0;

        return preset.Equalizer.Select(x => new EqualizerBand(i++, x.Gain)).ToArray();
    }
}