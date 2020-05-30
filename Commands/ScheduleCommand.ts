import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import AnnounceCommandBase from "./AnnounceCommandBase";
import CommandArgument from "../CommandArgument";
import { TextChannel } from "discord.js";
import Bot from "../Bot";

class ScheduleCommand extends AnnounceCommandBase {
    aliases: string[] = [ "schedule", "s" ];

    announce(announceMessage: string, announceChannel: TextChannel, command: CommandMessage): void {
        if (!command.arguments[1]) {
            command.message.reply("Please provide an ISO date to schedule the announcement for. (YYYY-MM-DD)");
        }

        let targetDate = new Date();

        try {
            targetDate = this.parseDate(command.arguments[1].content, command.arguments[2].content);
        }
        catch (error) {
            command.message.reply(`Invalid date and/or time: ${command.arguments[1]} ${command.arguments[2]}`);
        }

        const delay = targetDate.getTime() - new Date().getTime();

        if (delay < 30000) {
            command.message.reply("Cannot schedule announcement for less than 30 seconds.");
        }

        Bot.client.setTimeout(() => {
            announceChannel.send(announceMessage);
        }, delay);

        command.message.channel.send(`Scheduled announcement \`\`${announceMessage}\`\` for ${targetDate.toDateString()} ${targetDate.toLocaleTimeString("en-US")}`);
    }

    private parseDate(date: string, time: string) : Date {
        const day = new Date(date);
        const hours = parseInt(time.split(":")[0]);
        const minutes = parseInt(time.split(":")[1]);
        const dateTime = new Date(day.getFullYear(), day.getMonth(), day.getDate(), hours, minutes, 0, 0);
        return dateTime;
    }
}

export = ScheduleCommand;