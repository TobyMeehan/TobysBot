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

        const hexCode = command.arguments[0].content;

        if (!(/^#[0-9a-f]{6}?$/i.test(hexCode)) || hexCode != "clear") { // check if hexcode is a valid colour
            command.message.reply("Invalid hex colour. Use the format #xxxxxx.");
            return;
        }

        const colourRoles = command.message.guild?.roles.cache.filter(r => r.name.startsWith("#"));

        if (colourRoles) {
            command.message.member?.roles.remove(colourRoles);
        }

        if (hexCode === "clear") {
            return;
        }

        let role = colourRoles?.find(r => r.name === hexCode);

        if (!role) {
            role = await command.message.guild?.roles.create({
                data: {
                    name: hexCode,
                    color: hexCode,
                    hoist: false,
                    mentionable: false,
                    position: command.message.guild.roles.cache.array().length - 1
                }
            });
        }

        if (role) {
            command.message.member?.roles.add(role);
        }
    }
}

export = ColourmeCommand;