using System;
using System.Collections.Generic;

namespace SlimeEvolution.Core.Utilities;

public static class RandomExtensions
{
    public static T PickRandom<T>(this Random rng, IReadOnlyList<T> items)
    {
        if (items.Count == 0)
        {
            throw new InvalidOperationException("Cannot pick from an empty collection.");
        }

        return items[rng.Next(items.Count)];
    }

    public static List<T> TakeRandomSample<T>(this Random rng, IReadOnlyList<T> items, int count)
    {
        var output = new List<T>(Math.Min(count, items.Count));
        if (count <= 0 || items.Count == 0)
        {
            return output;
        }

        var available = new List<T>(items);
        for (var i = 0; i < count && available.Count > 0; i++)
        {
            var index = rng.Next(available.Count);
            output.Add(available[index]);
            available.RemoveAt(index);
        }

        return output;
    }
}
