using TobysBot.Configuration;
using TobysBot.Voice.Commands;

namespace TobysBot.Voice.Configuration;

public static class TobysBotBuilderExtensions
{
    public static TobysBotBuilder AddVoiceModule(this TobysBotBuilder builder)
    {
        builder.Services.AddVoiceModule();
        
        builder.Modules.AddModule<VoiceModule>();

        return builder;
    }
}