using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TobysBot.Discord.Audio;
using TobysBot.Discord.Client.TextCommands.Extensions;

namespace TobysBot.Discord.Client.TextCommands;

public abstract class VoiceModuleBase : ModuleBase<SocketCommandContext>
{
    private readonly IAudioNode _node;

    public VoiceModuleBase(IAudioNode node)
    {
        _node = node;
    }
    
    protected bool IsUserInVoiceChannel(out IVoiceState voiceState)
    {
        voiceState = Context.User as IVoiceState;
        return voiceState?.VoiceChannel is not null;
    }

    protected bool IsUserInSameVoiceChannel(out IVoiceState voiceState)
    {
        if (!IsUserInVoiceChannel(out voiceState))
        {
            return false;
        }

        return voiceState.VoiceChannel.Id == Context.Guild.CurrentUser.VoiceChannel?.Id;
    }

    protected async Task<bool> EnsureUserInVoiceAsync(bool joinVc = true)
    {
        if (!IsUserInVoiceChannel(out IVoiceState voiceState))
        {
            await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildJoinVoiceEmbed());
            return false;
        }

        if (joinVc)
        {
            await _node.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
        }

        return true;
    }

    protected async Task<bool> EnsureUserInSameVoiceAsync()
    {
        if (!IsUserInSameVoiceChannel(out IVoiceState voiceState))
        {
            await Context.Message.ReplyAsync(embed: new EmbedBuilder().BuildJoinSameVoiceEmbed());
            return false;
        }

        return true;
    }
}