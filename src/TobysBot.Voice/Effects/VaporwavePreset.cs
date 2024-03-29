﻿namespace TobysBot.Voice.Effects;

public class VaporwavePreset : BasePreset
{
    public override double Speed => 0.83;
    public override double Pitch => 0.89;

    public override IEqualizer Equalizer => new VaporwaveEqualizer();
}