import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import Bot from "../Bot";
import { User } from "discord.js";
import CommandArgument from "../CommandArgument";

class AutospamCommand implements ICommand {
    aliases: string[] = ["autosquare"]

    spams: Map<string, boolean> = new Map<string, boolean>();

    async execute(command: CommandMessage) {
        if (command.message.author.id != Bot.configuration.developerId) {
            return command.message.reply("Generic permission denied message.");
        }

        if (command.arguments[0].content == "stop") {
            this.stop(command);
        }
        else {
            this.start(command);
        }
    }

    async start(command: CommandMessage) {
        const user = command.arguments[0].mention;

        if (!user) {
            return command.message.reply("Cannot find user.");
        }

        console.log(`User is ${user.username}`);

        this.spams.set(user.id, true);

        while (this.spams.get(user.id)) {
            await user.send("square");
            console.log(`Sent square to ${user.username}`);
            await this.sleep(1000);
        }
    }

    stop(command: CommandMessage) {
        const user = command.arguments[1].mention;

        if (!user) {
            return command.message.reply("Cannot find user.");
        }

        this.spams.delete(user.id);

        console.log(`Autospam stopped to ${user.username}`);
    }

    sleep(ms: number) {
        return new Promise(x => setTimeout(x, ms));
    }
}

export = AutospamCommand;