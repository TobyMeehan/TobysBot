using Discord;

namespace TobysBot.Events;

public class VoiceStateUpdatedEventArgs
{
    public VoiceStateUpdatedEventArgs(IUser user, IVoiceState originVoiceState, IVoiceState currentVoiceState)
    {
        User = user;
        OriginVoiceState = originVoiceState;
        CurrentVoiceState = currentVoiceState;
    }

    public IUser User { get; }
    public IVoiceState OriginVoiceState { get; }
    public IVoiceState CurrentVoiceState { get; }
}