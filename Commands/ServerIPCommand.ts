import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import Bot from "../Bot";

class ServerIPCommand implements ICommand {
    aliases: string[] = ["serverip"];

    execute(command: CommandMessage): void {
        command.message.channel.send(`The current IP address of the minecraft server is \`\`${Bot.configuration.serverip}\`\`.`);
    }
}

export = ServerIPCommand;