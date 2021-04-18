import ICommand from "../ICommand";
import CommandMessage from "../CommandMessage";
import { searchByText, getPost, IPost } from "api-rule34-xxx";
import Random from "../Random";

class Rule34Command implements ICommand {
    aliases: string[] = ["rule34"];

    async execute(command: CommandMessage): Promise<void> {
        command.message.channel.startTyping();

        const list = await searchByText(command.arguments[0].content);
        let index = Math.floor(Random.next(0, list.length - 1));
        let post: IPost;

        do {
            post = await getPost(list[index].id);
            index = Math.floor(Random.next(0, list.length - 1));
        } while(!this.postIsImage(post));

        await command.message.channel.send(this.getEmbed(post));

        command.message.channel.stopTyping();
    }

    getEmbed(post: IPost) {
        const image = post.pages[0].imgURL[1];

        return {
            "embed": {
                "author": {
                    "name": "rule34.xxx",
                    "url": "https://rule34.xxx",
                    "icon_url": "https://rule34.xxx/apple-touch-icon-precomposed.png"
                },

                "color": 11199907,

                "image": {
                    "url": image
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