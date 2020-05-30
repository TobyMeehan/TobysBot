import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import HelpEmbed from "./HelpEmbed.json";
import CommandRegistry from "../CommandRegistry";

@CommandRegistry.register
class HelpCommand implements ICommand {
    aliases: string[] = ["help"];

    execute(command: CommandMessage): void {
        command.message.channel.send(HelpEmbed);
    }
}

export = HelpCommand;