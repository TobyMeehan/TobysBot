﻿using Discord;

namespace TobysBot.Voice.Extensions;

public static class UserExtensions
{
    public static bool IsInVoiceChannel(this IUser user, out IVoiceState voiceState)
    {
        voiceState = user as IVoiceState;

        return voiceState?.VoiceChannel is not null;
    }
}