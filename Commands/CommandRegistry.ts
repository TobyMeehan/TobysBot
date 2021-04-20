import ICommand from "../ICommand";
import AnnounceCommand from "./AnnounceCommand";
import ColourmeCommand from "./ColourmeCommand";
import HelpCommand from "./HelpCommand";
import PopCommand from "./PopCommand";
import ScheduleCommand from "./ScheduleCommand";
import ServerIPCommand from "./ServerIPCommand";
import SummonCommand from "./SummonCommand";
import StopCommand from "./StopCommand";
import JoinCommand from "./JoinCommand";
import LeaveCommand from "./LeaveCommand";
import ShutUpDuggan from "./ShutUpDuggan";
import AutospamCommand from "./AutospamCommand";
import UptimeCommand from "./UptimeCommand";
import ToggleCommand from "./ToggleCommand";
import TypicalCommand from "./TypicalCommand";
import Rule34Command from "./Rule34Command";
import WordcloudCommand from "./WordcloudCommand";

const Commands : ICommand[] = [
    new AnnounceCommand(),
    new ColourmeCommand(),
    new HelpCommand(),
    new PopCommand(),
    new ScheduleCommand(),
    new ServerIPCommand(),
    new StopCommand(),
    new SummonCommand(),
    new JoinCommand(),
    new LeaveCommand(),
    new ShutUpDuggan(),
    new AutospamCommand(),
    new UptimeCommand(),
    new ToggleCommand(),
    new TypicalCommand(),
    new Rule34Command(),
    new WordcloudCommand()
]

export = Commands;