import CommandMessage from "./CommandMessage";

interface ICommand {
    aliases: string[];

    execute(command: CommandMessage) : void;
}

export = ICommand;