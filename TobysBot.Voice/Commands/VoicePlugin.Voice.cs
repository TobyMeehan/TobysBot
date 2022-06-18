using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;

namespace TobysBot.Voice.Commands;

public partial class VoicePlugin
{
    public class VoiceModule : VoiceCommandModuleBase
    {
        private readonly IVoiceService _voiceService;
        private readonly EmbedService _embeds;

        private IEmote OkEmote => new Emoji("👌");
    
        public VoiceModule(IVoiceService voiceService, EmbedService embedService) : base(voiceService, embedService)
        {
            _voiceService = voiceService;
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
        [CheckVoice(sameChannel: SameChannel.Required)]
        public async Task RebindAsync(
            [Summary("Channel to rebind to.")]
            ITextChannel channel = null, 
            [Summary("Name of channel to rebind to.")]
            string channelname = null)
        {
            if (channelname is not null)
            {
                await Rebind(channelname);
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
            var channel = Context.Guild.TextChannels.FirstOrDefault(x => x.Name == channelName);

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

            await _voiceService.UpdateVolumeAsync(Context.Guild, (ushort)volume);

            await Response.ReactAsync(OkEmote);
        }
        
        [Command("bassboost")]
        [Summary("Applies a bass boost effect.")]
        [CheckVoice(sameChannel: SameChannel.Required)]
        public async Task BassBoostAsync(
            [Summary("Amount of bass boost, default is 0.")]
            double amount)
        {
            if (amount is < 0 or > 100)
            {
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("Bass boost must be between 0 - 100.")
                    .Build());
            }

            var multiplier = amount / 33;
            
            await _voiceService.UpdateBassBoostAsync(Context.Guild, multiplier);

            await Response.ReactAsync(OkEmote);
        }

        [Command("speed")]
        [Summary("Sets the playback speed.")]
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
                    await _voiceService.UpdateSpeedAsync(Context.Guild, speed);

                    await Response.ReactAsync(OkEmote);
                    break;
            }
        }

        [Command("pitch")]
        [Summary("Sets the pitch.")]
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
                    await _voiceService.UpdatePitchAsync(Context.Guild, pitch);

                    await Response.ReactAsync(OkEmote);
                    break;
            }
        }

        [Command("rotate")]
        [Summary("Adds a rotation effect.")]
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
            
            await _voiceService.UpdateRotationAsync(Context.Guild, speed);

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

        [Command("reset effects")]
        [Summary("Resets all audio effects.")]
        [CheckVoice(sameChannel: SameChannel.Required)]
        public async Task ResetEffectsAsync()
        {
            await _voiceService.ResetEffectsAsync(Context.Guild);

            await Response.ReactAsync(OkEmote);
        }
    }
}