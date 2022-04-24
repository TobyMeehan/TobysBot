namespace TobysBot.Discord.Audio.Status
{
    public interface ITrackStatus : IConnectedStatus
    {
        IActiveTrack CurrentTrack { get; }
        bool IsPaused { get; }
    }
}