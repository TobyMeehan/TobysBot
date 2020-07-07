import { VoiceConnection } from "discord.js";
import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import Bot from "../Bot";

class ShutUpDuggan implements ICommand {
    aliases: string[] = ["shutupduggan", "sud"]

    async execute(command: CommandMessage) {
        const channel = command.message.member?.voice.channel;
        const duggan = command.message.guild?.members.cache.get(Bot.configuration.dugganid);

        if (!channel?.members.some(m => m.id == duggan?.id)) {
            return command.message.channel.send(`Shut up ${duggan?.toString()}`);
        }

        const connection = await channel.join();

        if (!connection) {
            return;
        }

        const filename = `${process.cwd()}\\${this.pickFilename()}`;
        connection.play(filename);
    }

    pickFilename(): string {
        const random = Math.random() * 200;

        switch (true) {
            case random < 100: return "";
            case random < 110: return "SHUTUPDUGGAN.mp3";
            case random < 200: return "shut_up_duggan.mp3";
        }

        return "";
    }
}

export = ShutUpDuggan;