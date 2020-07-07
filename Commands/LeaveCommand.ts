import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";

class LeaveCommand implements ICommand {
    aliases: string[] = ["leave", "fuckoff"]

    async execute(command : CommandMessage) {
        await command.message.member?.voice.channel?.leave();
    }
}

export = LeaveCommand;