import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import Bot from "../Bot";

class SummonCommand implements ICommand {
    aliases: string[] = ["summon"];

    execute(command: CommandMessage): void {
        if (!command.arguments[0]) {
            command.message.reply("Consider yourself summoned.");
        }

        const user = command.arguments[0].mention;

        if (!user) {
            command.message.reply("I'm not sure who that is.");
        }

        switch (user?.id) {
            case Bot.client.user?.id:
                command.message.channel.send("Do not worry, I am here.");
                break;
            case command.message.author.id:
                command.message.reply("Consider yourself summoned.");
                break;
            default:
                command.message.channel.send(`${user?.toString()}, ${command.message.author.toString()} would like you to join them in their adventure.`);
                break;
        }

        command.message.delete();
    }
}

export = SummonCommand;