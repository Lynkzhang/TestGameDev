using System;
using System.Collections.Generic;
using System.Linq;

namespace SlimeEvolution.Core.Utilities;

public sealed class WeightedPicker<T>
{
    private readonly List<(T Item, double Weight)> _entries = new();

    public void Add(T item, double weight)
    {
        if (weight <= 0)
        {
            return;
        }

        _entries.Add((item, weight));
    }

    public bool TryPick(Random rng, out T? item)
    {
        if (_entries.Count == 0)
        {
            item = default;
            return false;
        }

        var totalWeight = _entries.Sum(e => e.Weight);
        var roll = rng.NextDouble() * totalWeight;
        double cumulative = 0;

        foreach (var entry in _entries)
        {
            cumulative += entry.Weight;
            if (roll <= cumulative)
            {
                item = entry.Item;
                return true;
            }
        }

        item = _entries[^1].Item;
        return true;
    }
}
