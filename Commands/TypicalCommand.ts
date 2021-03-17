import { TextChannel } from "discord.js";
import Bot from "../Bot";
import CommandMessage from "../CommandMessage";
import ICommand from "../ICommand";
import Random from "../Random";
import TypicalQuotes from "./TypicalQuotes.json";

class TypicalCommand implements ICommand {
     aliases: string[] = ["typical"]

     async execute(command: CommandMessage): Promise<void> {
         const channel: TextChannel = command.message.channel as TextChannel;
         const webhook = await channel.createWebhook("Typical");

         const quoteUser = Random.select(TypicalQuotes.users);
         const quote = Random.select(quoteUser.quotes);

         const user = await Bot.client.users.fetch(quoteUser.id);

         await webhook.send(quote, {
             username: user.username,
             avatarURL: user.displayAvatarURL()
         });

         await webhook.delete();
     }
}

export = TypicalCommand