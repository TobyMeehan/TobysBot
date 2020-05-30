import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import { Role } from "discord.js";

class ColourmeCommand implements ICommand {
    aliases: string[] = ["colourme"];

    async execute(command: CommandMessage): Promise<void> {
        if (!command.arguments[0]) {
            command.message.reply("Please provide a hex colour. (#xxxxxx)");
            return;
        }

        await command.message.member?.roles.remove(command.message.member.roles.cache.filter(r => r.name.startsWith("#")));

        if (command.arguments[0].content === "clear") {
            return;
        }

        const hexCode = command.arguments[0].content;

        if (!(/^#[0-9a-f]{6}?$/i.test(hexCode))) {
            command.message.reply("Invalid hex colour. Use the format #xxxxxx.");
            return;
        }

        let colourRole = command.message.guild?.roles.cache.find(r => r.name === hexCode) 
        
        if (!colourRole) {
            colourRole = await command.message.guild?.roles.add({
                name: hexCode,
                color: hexCode,
                hoist: false,
                mentionable: false
            }).setPosition(command.message.guild.roles.cache.array().length - 2);
        }

        if (colourRole) {
            command.message.member?.roles.add(colourRole);
        }
    }
}

export = ColourmeCommand;