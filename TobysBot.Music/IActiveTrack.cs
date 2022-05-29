namespace TobysBot.Music;

public interface IActiveTrack : ITrack
{
    TimeSpan Position { get; }
}