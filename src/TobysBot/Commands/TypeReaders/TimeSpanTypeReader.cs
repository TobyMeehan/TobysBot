using System.Globalization;
using Discord.Commands;

namespace TobysBot.Commands.TypeReaders;

public class TimeSpanTypeReader : TypeReader
{
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        string[] formats = { @"%h\:%m\:%s", @"%m\:%s" };

        if (!TimeSpan.TryParseExact(input, formats, null, TimeSpanStyles.None, out var result))
        {
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed,
                "Could not parse that timestamp."));
        }

        return Task.FromResult(TypeReaderResult.FromSuccess(result));
    }
}