using Discord.Commands;

namespace TobysBot.Commands;

public class EnumTypeReader<T> : TypeReader where T : struct, Enum
{
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        if (!Enum.TryParse(input, ignoreCase: true, out T result))
        {
            return Task.FromResult(
                TypeReaderResult.FromError(CommandError.ParseFailed, "Enum parse failed."));
        }

        return Task.FromResult(
            TypeReaderResult.FromSuccess(result));
    }
}