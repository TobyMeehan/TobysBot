using System;
using System.Collections.Generic;
using System.Linq;

namespace TobysBot.Discord.Client.TextCommands.Extensions;

public static class EnumerableExtensions
{
    public static T SelectRandom<T>(this IEnumerable<T> collection)
    {
        var list = collection.ToList();
        
        var index = Random.Shared.Next(0, list.Count);

        return list[index];
    }
}