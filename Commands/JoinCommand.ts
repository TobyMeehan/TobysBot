import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";

class JoinCommand implements ICommand {
    aliases: string[] = ["join"]

    async execute(command: CommandMessage) {
        const channel = command.message.member?.voice.channel;

        if (!channel) {
            return command.message.reply("You need to be in a voice channel.");
        }

        await channel.join();
    }
}

export = JoinCommand;