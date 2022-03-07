# Toby's Bot
My Discord bot, currently a rework of an older version so some of the original commands are missing, but it can now play music, so yay!

In general, it does not serve any specific purpose, just automating certain things I find myself needing it to. However, given the increase in popular music bots being _taken care of_, I have rewritten it to replicate many of their functionalities. For this reason, and also that I do not own the audio server it uses, I haven't made it public to invite, however I do publish docker images (see packages) if you want to host it yourself.

## Commands
Currently only music commands are working, the old ones will be back soon. The bot is also in an early stage, so help is not yet available as a command.

### `\join`
Joins your voice channel.

### `\leave`
Leaves the voice channel.

### `\play [query]`
Finds a track for the query and plays it, or resumes playback if paused.

### `\pause`
Pauses playback.

### `\skip`
Skips to the next track in the queue.

### `\stop`
Stops playback and returns to the start of the queue.

### `\clear`
Clears the queue.

### `loop [track|queue]`
Enables looping of the current track or queue.

### `\np`
Current track status.

### `\queue`
Current queue status.
