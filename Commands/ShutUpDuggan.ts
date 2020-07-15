import { VoiceConnection } from "discord.js";
import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import Bot from "../Bot";
import Random from "../Random";

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
        return Random.select("", "SHUTUPDUGGAN.mp3", "shut_up_duggan.mp3");
    }
}

export = ShutUpDuggan;