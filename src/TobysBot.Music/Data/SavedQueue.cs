using Discord;
using TobysBot.Data;

namespace TobysBot.Music.Data;

public class SavedQueue : Entity, ISavedQueue, IUserRelation, INamedEntity
{
    public SavedQueue()
    {
        
    }

    public SavedQueue(string name, IUser user, IQueue queue)
    {
        Name = name;
        UserId = user.Id;
        Tracks = queue.Select(x => new SavedTrack(x)).ToList();
    }

    // -- Data to write --
    
    public string Name { get; set; }
    public ulong UserId { get; set; }
    public List<SavedTrack> Tracks { get; set; }
    
    // -- --

    IEnumerable<ITrack> ISavedQueue.Tracks => Tracks;
    
}