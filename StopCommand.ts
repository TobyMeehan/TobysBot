import ICommand from "./ICommand";
import CommandMessage from "./CommandMessage";
import Bot from "./Bot";

class StopCommand implements ICommand {
    aliases: string[] = ["stop"];

    async execute(command: CommandMessage): Promise<void> {
        if (command.message.author.id != Bot.configuration.developerId) {
            return;
        }

        await command.message.channel.send("Stopping process. cya");

        process.exit();
    }
}

export = StopCommand;