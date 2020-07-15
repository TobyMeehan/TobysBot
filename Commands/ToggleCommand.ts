import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import Bot from "../Bot";

class ToggleCommand implements ICommand {
    aliases: string[] = ["toggle"];

    execute(command: CommandMessage) {
        Bot.toggleActivity();
    }
}

export = ToggleCommand;