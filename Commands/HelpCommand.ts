import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import HelpEmbed from "./HelpEmbed.json";

class HelpCommand implements ICommand {
    aliases: string[] = ["help"];

    execute(command: CommandMessage): void {
        command.message.channel.send(HelpEmbed);
    }
}

export = HelpCommand;