using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Voice.Data;
using TobysBot.Voice.Effects;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;

namespace TobysBot.Voice.Commands;

[Plugin("voice")]
public class VoiceModule : VoiceCommandModuleBase
{
    private readonly IVoiceService _voiceService;
    private readonly ISavedPresetDataService _savedPresets;
    private readonly EmbedService _embeds;

    private static IEmote OkEmote => new Emoji("👌");

    public VoiceModule(IVoiceService voiceService, ISavedPresetDataService savedPresets, EmbedService embedService) :
        base(voiceService)
    {
        _voiceService = voiceService;
        _savedPresets = savedPresets;
        _embeds = embedService;
    }

    [Command("join", RunMode = RunMode.Async)]
    [Summary("Joins the voice channel.")]
    [CheckVoice]
    public async Task JoinAsync()
    {
        await JoinVoiceChannelAsync();

        await Response.ReactAsync(OkEmote);
    }

    [Command("leave", RunMode = RunMode.Async)]
    [Alias("disconnect", "fuckoff")]
    [Summary("Leaves the voice channel.")]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task LeaveAsync()
    {
        await LeaveVoiceChannelAsync();

        await Response.ReactAsync(OkEmote);
    }

    [Command("rebind")]
    [Summary("Rebinds track notifications to the specified text channel.")]
    [Usage("rebind", "channel mention or name")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task RebindAsync(
        [Summary("Channel to rebind to.")] ITextChannel? channel = null,
        [Name("channelname")] [Summary("Name of channel to rebind to.")]
        string? channelName = null)
    {
        if (channelName is not null)
        {
            await Rebind(channelName);
            return;
        }

        if (channel is not null)
        {
            await Rebind(channel);
            return;
        }

        await Rebind();
    }

    private async Task Rebind()
    {
        if (Context.Channel is not ITextChannel channel)
        {
            return;
        }

        if (Status is not IConnectedStatus status)
        {
            return;
        }

        if (status.TextChannel.Id == channel.Id)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithAlreadyBoundError(channel)
                .Build());

            return;
        }

        await _voiceService.RebindChannelAsync(channel);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithRebindAction(channel)
            .Build());
    }

    private async Task Rebind(string channelName)
    {
        var channel = Context.Guild!.TextChannels.FirstOrDefault(x => x.Name == channelName);

        if (channel is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription($"Could not find text channel with name {channelName}.")
                .Build());

            return;
        }

        if (Status is not IConnectedStatus status)
        {
            return;
        }

        if (status.TextChannel.Id == channel.Id)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithAlreadyBoundError(channel)
                .Build());

            return;
        }

        await _voiceService.RebindChannelAsync(channel);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithRebindAction(channel)
            .Build());
    }

    private async Task Rebind(ITextChannel channel)
    {
        if (Status is not IConnectedStatus status)
        {
            return;
        }

        if (status.TextChannel.Id == channel.Id)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithAlreadyBoundError(channel)
                .Build());

            return;
        }

        await _voiceService.RebindChannelAsync(channel);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithRebindAction(channel)
            .Build());
    }

    // effects

    [Command("volume")]
    [Summary("Sets the volume.")]
    [Usage("volume", "new volume")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task VolumeAsync(
        [Summary("New volume, default is 100.")]
        int volume)
    {
        if (volume is < 0 or > 200)
        {
            await Response.ReplyAsync("Volume must be between 0 - 200.");
            return;
        }

        await _voiceService.UpdateVolumeAsync(Context.Guild!, (ushort)volume);

        await Response.ReactAsync(OkEmote);
    }

    [Command("bassboost")]
    [Summary("Applies a bass boost effect.")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task BassBoostAsync(
        [Summary("Amount of bass boost, default is 0.")]
        double amount)
    {
        if (amount is < 0 or > 200)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription("Bass boost must be between 0 - 200.")
                .Build());
        }

        double multiplier = amount / 40;

        await _voiceService.UpdateEqualizerAsync(Context.Guild!, new BassBoostEqualizer(multiplier));

        await Response.ReactAsync(OkEmote);
    }

    [Command("speed")]
    [Summary("Sets the playback speed.")]
    [Usage("speed", "new speed")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task SpeedAsync(
        [Summary("New speed multiplier, default is 1.")]
        double speed)
    {
        switch (speed)
        {
            case <= 0:
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("Speed must be greater than 0.")
                    .Build());

                return;
            case > 20:
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("That speed is too fast for me to handle! Twenty's plenty.")
                    .Build());

                return;
            default:
                await _voiceService.UpdateSpeedAsync(Context.Guild!, speed);

                await Response.ReactAsync(OkEmote);
                break;
        }
    }

    [Command("pitch")]
    [Summary("Sets the pitch.")]
    [Usage("pitch", "new pitch")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task PitchAsync(
        [Summary("New pitch multiplier, default is 1.")]
        double pitch)
    {
        switch (pitch)
        {
            case <= 0:
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("Pitch must be greater than 0.")
                    .Build());

                return;
            case > 100:
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("That pitch is inaudible! Best keep it under 100.")
                    .Build());

                return;
            default:
                await _voiceService.UpdatePitchAsync(Context.Guild!, pitch);

                await Response.ReactAsync(OkEmote);
                break;
        }
    }

    [Command("rotate")]
    [Summary("Adds a rotation effect.")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task RotateAsync(
        [Summary("Rotation speed in Hz, default is 0.")]
        double speed)
    {
        if (speed is < -60 or > 60)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription("If this track rotates any faster it'll take off! Maximum is 60Hz.")
                .Build());

            return;
        }

        await _voiceService.UpdateRotationAsync(Context.Guild!, speed);

        if (Random.Shared.Next(1, 1000) == 69)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Action)
                .WithDescription("It's time for WangerNumb, let's rotate the track!")
                .Build());

            return;
        }

        await Response.ReactAsync(OkEmote);
    }

    [Command("mono")]
    [Summary("Toggles mono (equal left and right channels).")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task MonoAsync()
    {
        var preset = await _voiceService.GetActivePresetAsync(Context.Guild!);

        if (preset.ChannelMix is MonoChannelMix)
        {
            await _voiceService.UpdateChannelMixAsync(Context.Guild!, new ChannelMix());
        }
        else
        {
            await _voiceService.UpdateChannelMixAsync(Context.Guild!, new MonoChannelMix());
        }

        await Response.ReactAsync(OkEmote);
    }

    [Command("nightcore")]
    [Summary("Toggles nightcore mode.")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task NightcoreAsync()
    {
        var active = await _voiceService.GetActivePresetAsync(Context.Guild!);

        if (active is NightcorePreset)
        {
            await _voiceService.RemoveActivePresetAsync(Context.Guild!);
        }
        else
        {
            await _voiceService.SetActivePresetAsync(Context.Guild!, new NightcorePreset());
        }

        await Response.ReactAsync(OkEmote);
    }

    [Command("vaporwave")]
    [Summary("Toggles vaporwave mode.")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task VaporwaveAsync()
    {
        var active = await _voiceService.GetActivePresetAsync(Context.Guild!);

        if (active is VaporwavePreset)
        {
            await _voiceService.RemoveActivePresetAsync(Context.Guild!);
        }
        else
        {
            await _voiceService.SetActivePresetAsync(Context.Guild!, new VaporwavePreset());
        }

        await Response.ReactAsync(OkEmote);
    }

    [Command("saved effects list")]
    [Summary("Lists all of your saved effect presets.")]
    public async Task ListSavedEffectsAsync()
    {
        var presets = await _savedPresets.ListSavedPresetsAsync(Context.User);

        if (!presets.Any())
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Information)
                .WithDescription("You do not have any saved effects. Use **/saved effects create** to create one.")
                .Build());

            return;
        }

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithSavedEffectListInformation(Context.User, presets)
            .Build());
    }

    [Command("saved effects create")]
    [Summary("Saves the current range of effects under the specified name.")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task CreateSavedEffectAsync(
        [Summary("Name of effect preset.")] string name)
    {
        var active = await _voiceService.GetActivePresetAsync(Context.Guild!);

        await _savedPresets.CreateSavedPresetAsync(name, Context.User, active);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithContext(EmbedContext.Action)
            .WithDescription($"Current preset saved to **{Format.Sanitize(name)}**")
            .Build());
    }

    [Command("saved effects delete")]
    [Summary("Deletes the specified saved effect preset.")]
    public async Task DeleteSavedEffectAsync(
        [Summary("Name of effect preset to delete.")]
        string name)
    {
        var savedPreset = await _savedPresets.GetSavedPresetAsync(Context.User, name);

        if (savedPreset is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithSavedPresetNotFoundError()
                .Build());

            return;
        }

        await _savedPresets.DeleteSavedPresetAsync(Context.User, name);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithContext(EmbedContext.Action)
            .WithDescription($"Saved effect preset **{savedPreset.Name}** deleted.")
            .Build());
    }

    [Command("saved effects load")]
    [Summary("Loads the specified saved effect preset.")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task LoadSavedEffectAsync(
        [Summary("Name of effect preset to load.")]
        string name)
    {
        var savedPreset = await _savedPresets.GetSavedPresetAsync(Context.User, name);

        if (savedPreset is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithSavedPresetNotFoundError()
                .Build());

            return;
        }

        await _voiceService.SetActivePresetAsync(Context.Guild!, savedPreset);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithContext(EmbedContext.Action)
            .WithDescription($"Loaded saved effect preset **{savedPreset.Name}**")
            .Build());
    }

    [Command("reset effects")]
    [Alias("rsfx")]
    [Summary("Resets all audio effects.")]
    [RequireContext(ContextType.Guild)]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task ResetEffectsAsync()
    {
        await _voiceService.ResetEffectsAsync(Context.Guild!);

        await Response.ReactAsync(OkEmote);
    }
}