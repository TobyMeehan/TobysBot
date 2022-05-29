namespace TobysBot.Music;

public interface ILoopSetting { }

public class DisabledLoopSetting : ILoopSetting { }

public class EnabledLoopSetting : ILoopSetting { }

public class TrackLoopSetting : EnabledLoopSetting { }

public class QueueLoopSetting : EnabledLoopSetting { }