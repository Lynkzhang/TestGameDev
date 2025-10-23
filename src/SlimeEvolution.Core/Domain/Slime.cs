using System;
using System.Collections.Generic;
using System.Linq;

namespace SlimeEvolution.Core.Domain;

public sealed class Slime
{
    private readonly List<TraitDefinition> _traits;
    private readonly List<SkillInstance> _skills;

    public Slime(
        Guid id,
        string name,
        int generation,
        Stats baseStats,
        IEnumerable<TraitDefinition>? traits = null,
        IEnumerable<SkillInstance>? skills = null,
        double baseSplitIntervalSeconds = 60,
        double baseMutationBonus = 0,
        AccessoryInstance? accessory = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Slime name cannot be empty.", nameof(name));
        }

        Id = id;
        Name = name;
        Generation = Math.Max(1, generation);
        BaseStats = baseStats;
        BaseSplitIntervalSeconds = baseSplitIntervalSeconds;
        BaseMutationBonus = baseMutationBonus;
        _traits = traits?.Distinct().ToList() ?? new List<TraitDefinition>();
        _skills = skills?.ToList() ?? new List<SkillInstance>();
        Accessory = accessory;
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public int Generation { get; }
    public Stats BaseStats { get; }
    public double BaseSplitIntervalSeconds { get; }
    public double BaseMutationBonus { get; }
    public AccessoryInstance? Accessory { get; private set; }

    public IReadOnlyList<TraitDefinition> Traits => _traits;
    public IReadOnlyList<SkillInstance> Skills => _skills;

    public void Rename(string newName)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            Name = newName;
        }
    }

    public void SetAccessory(AccessoryInstance? accessory) => Accessory = accessory;

    public EffectBundle CalculateAggregateEffects(EffectBundle environmentEffects)
    {
        var total = EffectBundle.Identity.Combine(environmentEffects);

        if (Accessory is { } accessory)
        {
            total = total.Combine(accessory.Definition.Effects);
        }

        foreach (var trait in _traits)
        {
            total = total.Combine(trait.Effects);
        }

        return total;
    }

    public Stats GetEffectiveStats(EffectBundle environmentEffects)
    {
        var aggregateEffects = CalculateAggregateEffects(environmentEffects);
        return BaseStats.ApplyEffects(aggregateEffects);
    }

    public bool HasTrait(string traitId) => _traits.Any(t => t.Id == traitId);
    public bool HasSkill(string skillId) => _skills.Any(s => s.Definition.Id == skillId);

    public Slime WithAdditionalTrait(TraitDefinition trait)
    {
        var traits = _traits.ToList();
        if (!traits.Contains(trait))
        {
            traits.Add(trait);
        }

        return new Slime(Id, Name, Generation, BaseStats, traits, _skills, BaseSplitIntervalSeconds, BaseMutationBonus, Accessory);
    }
}
