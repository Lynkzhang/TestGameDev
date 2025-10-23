using System.Collections.Generic;

namespace SlimeEvolution.Core.Domain;

public sealed class AccessoryDefinition
{
    public AccessoryDefinition(
        string id,
        string name,
        string description,
        AccessoryType type,
        EffectBundle effects,
        IReadOnlyCollection<string>? favoredTraitTags = null,
        SkillType? favoredSkillType = null)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
        Effects = effects;
        FavoredTraitTags = favoredTraitTags is null ? new HashSet<string>() : new HashSet<string>(favoredTraitTags);
        FavoredSkillType = favoredSkillType;
    }

    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public AccessoryType Type { get; }
    public EffectBundle Effects { get; }
    public IReadOnlyCollection<string> FavoredTraitTags { get; }
    public SkillType? FavoredSkillType { get; }
}

public sealed record AccessoryInstance(AccessoryDefinition Definition, bool IsSpecial = false);
