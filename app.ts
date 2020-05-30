import { Client } from "discord.js";
import Bot from "./Bot";
import Authentication from "./Authentication.json";
import CommandMessage from "./CommandMessage";

import Commands from "./Commands/CommandRegistry";

Bot.debugMode = true;
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