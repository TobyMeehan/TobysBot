import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import Bot from "../Bot";
import { TextChannel } from "discord.js";
import { inherits } from "util";
import AnnounceCommandBase from "./AnnounceCommandBase";
import CommandArgument from "../CommandArgument";

class AnnounceCommand extends AnnounceCommandBase {
    aliases: string[] = ["announce", "a"];

    announce(announceMessage: string, announceChannel: TextChannel, command: CommandMessage): void {
        announceChannel.send(announceMessage);
    }
}

export = AnnounceCommand;