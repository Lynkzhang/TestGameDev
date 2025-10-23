using System.Collections.Generic;

namespace SlimeEvolution.Core.Domain;

public sealed class SkillDefinition
{
    public SkillDefinition(
        string id,
        string name,
        string description,
        SkillType type,
        SkillRarity rarity,
        double power,
        double cooldownSeconds,
        IReadOnlyCollection<string>? tags = null)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
        Rarity = rarity;
        Power = power;
        CooldownSeconds = cooldownSeconds;
        Tags = tags is null ? new HashSet<string>() : new HashSet<string>(tags);
    }

    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public SkillType Type { get; }
    public SkillRarity Rarity { get; }
    public double Power { get; }
    public double CooldownSeconds { get; }
    public IReadOnlyCollection<string> Tags { get; }
}

public sealed record SkillInstance(SkillDefinition Definition, int Level = 1);
