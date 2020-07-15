import { Client } from "discord.js";
import Config from "./Configuration.json";
import DebugConfig from "./Configuration.Debug.json";
import Auth from "./Authentication.json";
import Random from "./Random";

/**
 * Represents the current bot.
 */
class Bot {
    static debugMode = false;
    static prefix = "";
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

    static toggleActivity() {
        const prefixActivity = Bot.prefix;
        const uptimeActivity = `${this.getUptimeString(new Date(Bot.uptime * 1000))} without an incident.`;
        const githubActivity = `https://github.com/TobyMeehan/TobysBot`;

        const activity = Random.select(prefixActivity, uptimeActivity, githubActivity);

        switch (activity) {
            case prefixActivity:
                Bot.client.user?.setActivity(activity, { type: "LISTENING" });
                break;
            case uptimeActivity:
                Bot.client.user?.setActivity(activity, { type: "PLAYING" });
                break;
            case githubActivity:
                Bot.client.user?.setActivity(activity, { type: "PLAYING" });
                break;
        }
    }

    private static getUptimeString(date: Date) {
        const hours = date.getHours() - 1;
        const minutes = date.getMinutes();
        const seconds = date.getSeconds();
        const days = Math.floor(hours / 24);

        if (days > 0) {
            return `${days} days`;
        }

        if (hours > 0) {
            return `${hours} hours`;
        }

        if (minutes > 0) {
            return `${minutes} minutes`;
        }

        return `${seconds} seconds`;
    }

    static uptime = 0;
}

export = Bot;