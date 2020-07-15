import { Client, Message } from "discord.js";
import Bot from "./Bot";
import Authentication from "./Authentication.json";
import CommandMessage from "./CommandMessage";
import Commands from "./Commands/CommandRegistry";
import ShutUpDuggan from "./Commands/ShutUpDuggan";
import * as https from "https";
import Timeout from "./Timeout";

Bot.prefix = "\\";
Bot.debugMode = false;
Bot.login();

let keepAlive = true;

Bot.client.on("ready", async () => {
    Bot.client.user?.setActivity(Bot.prefix, { type: "LISTENING" });

    while (keepAlive) {
        Bot.uptime++;

        if (Bot.uptime % (5 * 60) === 0) { // send every 5 mins
            Bot.toggleActivity();
            https.get("https://bot.tobymeehan.com");
        }

        await Timeout.sleep(1000);
    }
});

Bot.client.on("message", message => {

    if (!message.content.startsWith(Bot.prefix)) {
        return;
    }

    const command = new CommandMessage(Bot.prefix, message);
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

    command?.execute(new CommandMessage(Bot.prefix, message));
});

