import { User } from "discord.js";
import Bot from "./Bot";

class CommandArgument {
    content: string = "";
    mention: User | undefined;

    constructor (argument: string) {
        this.content = argument;
    }

    private getUserFromMention(mention: string) : User | undefined {
        if (mention.startsWith('<@') && mention.endsWith('>')) {
            mention = mention.slice(2, -1);
        }

        if (mention.startsWith("!")) {
            mention = mention.slice(1);
        }

        return Bot.client.users.cache.get(mention);
    }
}

export = CommandArgument;