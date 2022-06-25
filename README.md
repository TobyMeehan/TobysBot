# Toby's Bot

Toby's Bot is a modular, extensible, free and open source discord bot. Currently it mainly offers music functionality, however further extension is planned into custom commands, moderation and economy. Being open source, there is no premium so every feature (including many which would have required it previously) is available free of charge for all users.

## Features

**Music**
- Player commands (play, pause, seek, fast forward)
- Queue with many important features often missed, including remove, jump, loop and shuffle
- Lyrics
- Audio effects (pitch, speed, bassboost etc)
- Saved queues
- Support for YouTube, Spotify, Soundcloud and more

A full list of commands is available at [bot.tobymeehan.com/commands](https://bot.tobymeehan.com/commands), you can also use `/help` for an overview. The default prefix for text commands is `\` but this can be changed for your server.

## Invite
You can add Toby's Bot to your server at [bot.tobymeehan.com/invite](https://bot.tobymeehan.com/invite). Alternatively if you would like to self-host the bot you can use the docker images published for every release. See [Hosting](#hosting) for more information.

## Hosting
Currently the only supported way to host Toby's Bot is with the docker images, which include the web app, published every release. I plan to create a CLI app in the future which will support docker and a standalone binary. All images can be found in the [docker registry](https://github.com/TobyMeehan/TobysBot/pkgs/container/tobysbot) for this repository. In addition to the docker image, a MongoDB database and Lavalink server are needed.

### Lavalink
Toby's Bot requires a Lavalink server to play music. It is recommended to use the [docker image](https://hub.docker.com/r/fredboat/lavalink/) for this. You will also need to setup an application.yml to match your bot config. See my own [Lavalink Host](https://github.com/TobyMeehan/LavalinkHost/blob/master/application.yml) for an example, which is used for Toby's Bot.

### Database
A database is only required to use custom prefixes and saved queues and effect presets. Currently only a MongoDB client has been implemented, though I will consider adding more.

### Configuration
Some configuration is required before running the bot, this can be done with environment variables, or appsettings.json file. An example appsettings is shown below.

```json
"Authorization": {
  "Token": "token" // Discord bot token
},

"Prefix": "!", // Default text command prefix
  
"StartupStatus": "Starting...", // Status to show while bot is starting up

"Embeds": {
  // Colours to show for various embed contexts.
  "Colors": { 
    "Action": 5832704,
    "Information": 3278636,
    "Error": 789332
  }
},

"Data": {
  "GuildCollection": "guilds" // Name of database table or collection with guild info
},

"Voice": {
  // Settings for lavalink, see lavalink section
  "Lavalink": { 
    "Hostname": "example.lavalink.server",
    "Authorization": "lavalink password",
    "Port": 2333
  },

  "Data": {
    "SavedPresetCollection": "presets" // Name of database table or collection with saved preset info
  }
},

"Music": {
  "Data": {
    "SavedQueueCollection": "queues" // Name of database table or collection with saved queue info
  },
  
  // Spotify API credentials for loading links
  "Spotify": { 
    "ClientId": "spotify-client-id",
    "ClientSecret": "spotify-client-secret"
  }
},

// Settings for MongoDB
"Mongo": { 
  "ConnectionString": "mongodb-connection-string",
  "DatabaseName": "mongodb-database-name"
}
```
To add this configuration to the container, create a new Dockerfile and copy the file into the /app directory of the image.
```Dockerfile
FROM ghcr.io/tobymeehan/tobysbot:latest
WORKDIR /app
COPY appsettings.json ./appsettings.json
```
Environment variables follow the same structure as the json file, using `__` to represent a block, and prefixed with `TOBYSBOT_`.
```
TOBYSBOT_Prefix=!
TOBYSBOT_Authorization__Token=token
```
___

Toby's Bot is built using .NET 6 (with [Discord.NET](https://github.com/discord-net/Discord.Net) and [Victoria](https://github.com/Yucked/Victoria)), and [Lavalink](https://github.com/freyacodes/Lavalink) for the audio server.
