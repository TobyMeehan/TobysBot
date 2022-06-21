using TobysBot.Data;
using TobysBot.Voice.Effects;

namespace TobysBot.Voice.Data;

public class SavedPreset : Entity, ISavedPreset
{
    public SavedPreset()
    {
        
    }

    public SavedPreset(string name, ulong user, IPreset preset)
    {
        Name = name;
        UserId = user;
        Speed = preset.Speed;
        Pitch = preset.Pitch;
        Rotation = preset.Rotation;
        Equalizer = preset.Equalizer.ToList();
    }
    
    public string Name { get; set; } = null!;
    public ulong UserId { get; set; }
    public double Speed { get; set; }
    public double Pitch { get; set; }
    public double Rotation { get; set; }
    public List<Band> Equalizer { get; set; } = new();
    
    IEqualizer IPreset.Equalizer => new Equalizer(Equalizer);
}