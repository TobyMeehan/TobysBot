import ICommand from "../ICommand";
import AnnounceCommand from "./AnnounceCommand";
import ColourmeCommand from "./ColourmeCommand";
import HelpCommand from "./HelpCommand";
import PopCommand from "./PopCommand";
import ScheduleCommand from "./ScheduleCommand";
import ServerIPCommand from "./ServerIPCommand";
import SummonCommand from "./SummonCommand";
import StopCommand from "./StopCommand";

const Commands : ICommand[] = [
    new AnnounceCommand(),
    new ColourmeCommand(),
    new HelpCommand(),
    new PopCommand(),
    new ScheduleCommand(),
    new ServerIPCommand(),
    new StopCommand(),
    new SummonCommand()
]

export = Commands;