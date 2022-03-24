using System;

namespace TobysBot.Discord.Client.TextCommands;

public class HelpCategoryAttribute : Attribute
{
    public HelpCategoryAttribute(string name)
    {
        Name = name;
    }
    
    public string Name { get; set; }
}