import { Client, Message } from "discord.js";
import Bot from "./Bot";
import Authentication from "./Authentication.json";
import CommandMessage from "./CommandMessage";

import Commands from "./Commands/CommandRegistry";
import ShutUpDuggan from "./Commands/ShutUpDuggan";

Bot.debugMode = false;
Bot.login();

const prefix = "\\";

Bot.client.on("ready", () => {
    Bot.client.user?.setActivity(prefix, { type: "LISTENING" });
});

Bot.client.on("message", message => {

    if (!message.content.startsWith(prefix)) {
        return;
    }

    const command = new CommandMessage(prefix, message);
    Commands.find(c => c.aliases.includes(command.command))?.execute(command);
});

Bot.client.on("guildMemberSpeaking", async (member, speaking) => {
    const command = Commands.find(c => c instanceof ShutUpDuggan);

    if (member.id != Bot.configuration.dugganid) {
        return;
    }

    const message = await await member.guild.systemChannel?.send("Shut up Duggan");

    if (!message) {
        return;
    }

    command?.execute(new CommandMessage(prefix, message));
});