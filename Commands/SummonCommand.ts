import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import Bot from "../Bot";

class SummonCommand implements ICommand {
    aliases: string[] = ["summon"];

    execute(command: CommandMessage): void {
        if (!command.arguments[0]) {
            command.message.reply("Consider yourself summoned.");
        }

        const author = command.message.author;
        const member = command.message.member;
        const user = command.arguments[0].mention;

        if (!user) {
            command.message.reply("I'm not sure who that is.");
        }

        switch (user?.id) {
            case Bot.client.user?.id:
                command.message.channel.send("Do not worry, I am here.");
                break;
            case author.id:
                command.message.reply("Consider yourself summoned.");
                break;
            default:
                if (author.presence.activities.length ?? 0 > 0) {
                    const activity = author.presence.activities[0];
                    command.message.channel.send(`${user?.toString()}, ${author.toString()} wants you to join them in ${activity.name}.`);
                    break;
                }

                if (member?.voice) {
                    command.message.channel.send(`${user?.toString()}, ${author.toString()} wants you to join them in \`${member.voice.channel?.name}\``);
                    break;
                }

                command.message.channel.send(`${user?.toString()}, ${author.toString()} wants you to join them in whatever they are doing.`);
                break;
        }

        command.message.delete();
    }
}

export = SummonCommand;