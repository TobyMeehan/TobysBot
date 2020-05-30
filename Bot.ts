import { Client } from "discord.js";
import Config from "./Configuration.json";
import DebugConfig from "./Configuration.Debug.json";
import Auth from "./Authentication.json";

/**
 * Represents the current bot.
 */
class Bot {
    static debugMode = false;
    static client: Client = new Client();

    static get configuration() {
        return Bot.debugMode ? DebugConfig : Config;
    }

    static get token(): string {
        return Bot.debugMode ? Auth.debugtoken : Auth.token;
    }

    static login() {
        Bot.client.login(Bot.token);
    }
}

export = Bot;