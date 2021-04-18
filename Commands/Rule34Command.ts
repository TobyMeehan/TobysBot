import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import { searchByText, getPost, IPost } from "api-rule34-xxx";
import Random from "../Random";
import { Message } from "discord.js";

class Rule34Command implements ICommand {
    aliases: string[] = ["rule34"];

    async execute(command: CommandMessage): Promise<void> {
        command.message.channel.startTyping();

        const list = await searchByText(command.argument);

        if (list.length === 0) {
            command.message.reply("No results for that query");;
            command.message.channel.stopTyping();
            return;
        }

        for (let i = 0; i < 10; i++) { // try 10 times to find a suitable image
            const index = Math.floor(Random.next(0, list.length - 1))
            const post = await getPost(list[index].id);

            if (this.postIsImage(post)) {
                await command.message.channel.send(this.getEmbed(post, command.message));
                command.message.channel.stopTyping();
                return;
            }
        }

        command.message.reply("No image results for that query");

        command.message.channel.stopTyping();
    }

    getEmbed(post: IPost, message: Message) {
        const image = post.pages[0].imgURL[1];

        return {
            "embed": {
                "color": 11199907,
                
                "author": {
                    "name": "rule34.xxx",
                    "url": "https://rule34.xxx",
                    "icon_url": "https://rule34.xxx/apple-touch-icon-precomposed.png"
                },

                "title": "Original Post",
                "url": `https://rule34.xxx/index.php?s=view&page=post&id=${post.id}`,

                "fields": [
                    {
                        "name": "Tags",
                        "value": post.tags.map(x => x.name).join(", ")
                    }
                ],

                "image": {
                    "url": image
                },

                "footer": {
                    "icon_url": message.author.displayAvatarURL(),
                    "text": `Requested by ${message.author.username}#${message.author.discriminator}`
                }
            }
        }
    }

    postIsImage(post: IPost) {
        const url = post.pages[0].imgURL[1];

        if (url.includes(".jpg") || url.includes(".jpeg") || url.includes(".png") || url.includes(".gif")) {
            return true;
        }

        return false;
    }
}

export = Rule34Command;