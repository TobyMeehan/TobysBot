﻿namespace TobysBot.Voice.Effects;

public abstract class BasePreset : IPreset
{
    public virtual IEqualizer Equalizer => new Equalizer();
    public virtual double Speed => 1;
    public virtual double Pitch => 1;
    public virtual double Rotation => 0;
    public IChannelMix ChannelMix => new ChannelMix();
}