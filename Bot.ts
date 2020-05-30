import { Client } from "discord.js";
import Config from "./Configuration.json";

/**
 * Represents the current bot.
 */
class Bot {
    static client: Client;
    static configuration = Config;
}

export = Bot;