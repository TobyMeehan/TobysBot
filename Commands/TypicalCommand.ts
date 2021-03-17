import { TextChannel } from "discord.js";
import CommandMessage from "../CommandMessage";
import ICommand from "../ICommand";
import Random from "../Random";
import Timeout from "../Timeout";
import TypicalQuotes from "./TypicalQuotes.json";

class TypicalCommand implements ICommand {
     aliases: string[] = ["typical"]

     async execute(command: CommandMessage): Promise<void> {
         const channel: TextChannel = command.message.channel as TextChannel;
         const webhook = await channel.createWebhook("Typical");

         const user = Random.select(TypicalQuotes.users);
         const quote = Random.select(user.quotes);

         const member = await channel.guild.members.fetch(user.id);

         await webhook.send(quote, {
             username: member.nickname ?? member.user.username,
             avatarURL: member.user.displayAvatarURL()
         });

         await webhook.delete();

         const message = await channel.send("I need suggestions to make this command more interesting. If you have any ideas, send them in.");
         await Timeout.sleep(10000);
         await message.delete();
     }
}

export = TypicalCommand