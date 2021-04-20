import axios from "axios";
import { MessageAttachment } from "discord.js";
import CommandMessage from "../CommandMessage";
import ICommand from "../ICommand";
import * as fs from "fs";


class WordcloudCommand implements ICommand {
    aliases: string[] = ["wordcloud", "wc"];

    async execute(command: CommandMessage): Promise<void> {
        const channel = command.message.channel;

        channel.startTyping();

        let history = (await channel.messages.fetch()).array().join(" ");
        history = history.replace(/[^\w\s]/g, " ").replace(/\s+/g, " ");

        const response = await axios.post("https://quickchart.io/wordcloud", {
            text: history,
            format: "png",
            width: 900,
            height: 600,
            backgroundColor: "#000000",
            fontFamily: "sans-serif",
            scale: "linear",
            rotation: 0,
            maxNumWords: 1000,
            minWordLength: 4,
            case: "none",
            removeStopwords: true
        }, {
            responseType: 'arraybuffer'
        });

        const attachment = new MessageAttachment(Buffer.from(response.data, 'binary'), "wordcloud.png");

        channel.send(attachment);

        channel.stopTyping();
    }
}

export = WordcloudCommand;