﻿using Discord.WebSocket;
using TobysBot.Events;
using TobysBot.Voice.Lavalink;

namespace TobysBot.Voice.Events;

public class VoiceStateUpdatedEventHandler : IEventHandler<VoiceStateUpdatedEventArgs>
{
    private readonly DiscordSocketClient _client;
    private readonly ILavalinkNode _lavaNode;
    private readonly IEventService _events;

    public VoiceStateUpdatedEventHandler(DiscordSocketClient client, ILavalinkNode lavaNode, IEventService events)
    {
        _client = client;
        _lavaNode = lavaNode;
        _events = events;
    }
    
    public async Task HandleAsync(VoiceStateUpdatedEventArgs args)
    {
        if (args.User.Id != _client.CurrentUser.Id)
        {
            return;
        }
        
        if (args is { OriginVoiceState.VoiceChannel: { } channel, CurrentVoiceState.VoiceChannel: null })
        {
            var player = _lavaNode.GetPlayer(channel.Guild);
            
            if (player is null)
            {
                return;
            }

            if (player.VoiceChannel is { } playerChannel && playerChannel.Id != channel.Id)
            {
                return;
            }

            if (player.VoiceChannel?.Id == channel.Id)
            {
                await _lavaNode.LeaveAsync(channel);
            }
            
            await _events.InvokeAsync(new VoiceChannelLeaveEventArgs(channel, channel.Guild));
        }
    }
}