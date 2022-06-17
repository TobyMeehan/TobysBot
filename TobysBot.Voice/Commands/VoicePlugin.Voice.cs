using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Voice.Extensions;

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

        // effects

        [Command("volume")]
        [Summary("Sets the volume.")]
        [CheckVoice(sameChannel: SameChannel.Required)]
        public async Task VolumeAsync(
            [Summary("New volume.")]
            [Choice("reset", Value = 100)]
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
            [Summary("Amount of bass boost.")]
            [Choice("reset", Value = 0d)]
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
            [Summary("New speed multiplier.")] 
            [Choice("reset", Value = 1d)]
            double speed)
        {
            if (speed <= 0)
            {
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("Speed must be greater than 0.")
                    .Build());
                
                return;
            }
            
            await _voiceService.UpdateSpeedAsync(Context.Guild, speed);

            await Response.ReactAsync(OkEmote);
        }

        [Command("pitch")]
        [Summary("Sets the pitch.")]
        [CheckVoice(sameChannel: SameChannel.Required)]
        public async Task PitchAsync(
            [Summary("New pitch multiplier.")] 
            [Choice("reset", Value = 1d)]
            double pitch)
        {
            if (pitch <= 0)
            {
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("Pitch must be greater than 0.")
                    .Build());
                
                return;
            }
            
            await _voiceService.UpdatePitchAsync(Context.Guild, pitch);

            await Response.ReactAsync(OkEmote);
        }

        [Command("rotate")]
        [Summary("Adds a rotation effect.")]
        [CheckVoice(sameChannel: SameChannel.Required)]
        public async Task RotateAsync(
            [Summary("Rotation speed in Hz.")] 
            [Choice("reset", Value = 0d)]
            double speed)
        {
            await _voiceService.UpdateRotationAsync(Context.Guild, speed);

            await Response.ReactAsync(OkEmote);
        }
    }
}