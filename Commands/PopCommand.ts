import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import Bot from "../Bot";

class PopCommand implements ICommand {
    aliases: string[] = ["pop"];

    async execute(command: CommandMessage): Promise<void> {
        const argument = command.arguments[0];

        if (!argument) {
            await command.message.reply("You are a pop pop head.");
            return;
        }

        const user = argument.mention;

        if (!user) {
            await command.message.reply("I'm not sure who that is.");
            return;
        }

        switch (user.id) {
            case Bot.client.user?.id:
                await command.message.reply("I am not a pop pop head, how dare you!");
                break;
            case Bot.configuration.developerId:
                await command.message.channel.send(`${user.toString()} is a star. S T A R  S T A R`);
                break;
            default:
                await command.message.channel.send(`${user.toString()} is a pop pop head.`);
        }

        command.message.delete();
    }
}

export = PopCommand;