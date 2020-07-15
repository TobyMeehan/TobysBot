import { Client, Message } from "discord.js";
import Bot from "./Bot";
import Authentication from "./Authentication.json";
import CommandMessage from "./CommandMessage";
import Commands from "./Commands/CommandRegistry";
import ShutUpDuggan from "./Commands/ShutUpDuggan";
import * as https from "https";
import Timeout from "./Timeout";

Bot.debugMode = true;
Bot.login();

const prefix = "\\";

let keepAlive = true;

Bot.client.on("ready", async () => {
    Bot.client.user?.setActivity(prefix, { type: "LISTENING" });

    while (keepAlive) {
        Bot.uptime++;

        if (Bot.uptime % 300 === 0) { // send every 5 mins
            https.get("https://bot.tobymeehan.com");
        }

        await Timeout.sleep(1000);
    }
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

    const message = await member.guild.systemChannel?.send("Shut up Duggan");

    if (!message) {
        return;
    }

    command?.execute(new CommandMessage(prefix, message));
});

