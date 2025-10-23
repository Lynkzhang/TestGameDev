using System.Collections.Generic;

namespace SlimeEvolution.Core.Domain;

public sealed class TraitDefinition
{
    public TraitDefinition(
        string id,
        string name,
        string description,
        TraitRarity rarity,
        EffectBundle effects,
        IReadOnlyCollection<string>? tags = null)
    {
        Id = id;
        Name = name;
        Description = description;
        Rarity = rarity;
        Effects = effects;
        Tags = tags is null ? new HashSet<string>() : new HashSet<string>(tags);
    }

    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public TraitRarity Rarity { get; }
    public EffectBundle Effects { get; }
    public IReadOnlyCollection<string> Tags { get; }
}

public sealed record TraitInstance(TraitDefinition Definition);
