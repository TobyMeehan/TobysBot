import { Message } from "discord.js";
import CommandArgument from "./CommandArgument";

class CommandMessage {

    /**
     * The action to perform.
     */
    command: string = "";

    /**
     * The operand of the command.
     */
    argument: string = "";

    /**
     * The space-delimited command arguments.
     */
    arguments: CommandArgument[] = new Array();

    /**
     * The message object representing the command.
     */
    message: Message;

    constructor(prefix: string, message: Message) {
        this.message = message;
        let args = message.content.slice(prefix.length).trim().split(" ");
        this.command = args[0];
        args = args.slice(1);

        for (const arg of args) {
            this.arguments.push(new CommandArgument(arg));
            this.argument += `${arg}`;
        }

        this.argument = this.argument.trim();
    }
}

export = CommandMessage;