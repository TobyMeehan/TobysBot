import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import Bot from "../Bot";

class UptimeCommand implements ICommand {
    aliases: string[] = ["uptime"];

    execute(command: CommandMessage) {
        const date = new Date(Bot.uptime * 1000);

        command.message.channel.send(this.getDateString(date));
    }

    getDateString(date: Date): string {
        const hours = date.getHours() - 1;
        const minutes = date.getMinutes();
        const seconds = date.getSeconds();
        const days = Math.floor(hours / 24);

        let dateString = "It has been ";

        switch (true) {
            case days > 0:
                dateString += `${days} days and ${hours} hours`;
                break;
            case hours > 0:
                dateString += `${hours} hours, ${minutes} minutes`;
                break;
            case minutes > 0:
                dateString += `${minutes} minutes, ${seconds} seconds`;
                break;
            default:
                dateString += `${seconds} seconds`;
                break;
        }

        dateString += " since I was last offline.";

        return dateString;
    }
}

export = UptimeCommand;