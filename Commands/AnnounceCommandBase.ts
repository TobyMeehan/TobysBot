import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import { GuildMember, TextChannel, Base } from "discord.js";
import Bot from "../Bot";
import CommandArgument from "../CommandArgument";

abstract class AnnounceCommandBase implements ICommand {
    abstract aliases: string[];

    async execute(command: CommandMessage): Promise<void> {
        if (!this.isAuthorised(command.message?.member)) {
            command.message.reply("You do not have access to this command, blame it on the Duggernment.");
            return;
        }

        if (!command.arguments[0]) {
            command.message.reply("Please provide an announcement identifier. (<porn|civ>)");
            return;
        }

        let announceMessage = this.getMessage(command.arguments[0].content);

        if (!announceMessage) {
            command.message.reply(`Unknown announcement identifier: ${command.arguments[0].content}`);
        }

        const announceRole = command.message.guild?.roles.cache.get(Bot.configuration.announceRoleId);

        announceMessage = `${announceRole?.toString()}, ${announceMessage}`;

        const announceChannel = command.message.guild?.channels.cache.get(Bot.configuration.announceChannelId) as TextChannel;

        this.announce(announceMessage, announceChannel, command);
    }

    abstract announce(announceMessage: string, announceChannel: TextChannel, command: CommandMessage) : void;

    private isAuthorised(member: GuildMember | null | undefined) : boolean {
        return (member?.hasPermission("ADMINISTRATOR") || member?.id === Bot.configuration.developerId) ?? false;
    }

    private getMessage(id: string): string | undefined {
        switch (id) {
            case "porn":
            case "p":
                return "Playing Online Random Nonsense (P.O.R.N.) night is now beginning.";
            case "civ":
            case "c":
                return "CIV 5 Sunday is now beginning.";
        }
    }
}

export = AnnounceCommandBase;