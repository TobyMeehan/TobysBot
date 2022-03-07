namespace TobysBot.Discord.Audio;

public abstract class LoopSetting { }
public class DisabledLoopSetting : LoopSetting { }

public class EnabledLoopSetting : LoopSetting { }

public class TrackLoopSetting : EnabledLoopSetting { }

public class QueueLoopSetting : EnabledLoopSetting { }