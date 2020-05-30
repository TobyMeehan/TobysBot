import { Client } from "discord.js";
import Bot from "./Bot";
import Authentication from "./Authentication.json";
import CommandRegistry from "./CommandRegistry";
import CommandMessage from "./CommandMessage";

Bot.client = new Client();
Bot.client.login(Authentication.token);

const commands = CommandRegistry.getImplementations().map(x => new x());
const prefix = "\\";

Bot.client.on("ready", () => {
    Bot.client.user?.setActivity(prefix, { type: "LISTENING" });
});

Bot.client.on("message", message => {

    if (!message.content.startsWith(prefix)) {
        return;
    }

    const command = new CommandMessage(prefix, message);
    commands.find(c => c.aliases.includes(command.command))?.execute(command);
});