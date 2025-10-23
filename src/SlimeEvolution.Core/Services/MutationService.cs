using System;
using System.Collections.Generic;
using System.Linq;
using SlimeEvolution.Core.Configuration;
using SlimeEvolution.Core.Domain;
using SlimeEvolution.Core.Utilities;

namespace SlimeEvolution.Core.Services;

public sealed class MutationService
{
    private readonly GameBalanceConfig _config;

    public MutationService(GameBalanceConfig config)
    {
        _config = config;
    }

    public Slime CreateStarterSlime(string name, Random rng)
    {
        var stats = _config.StarterStatDistribution.Roll(rng);
        var traits = new List<TraitDefinition>();

        var commonTraits = _config.Traits.Where(t => t.Rarity <= TraitRarity.Common).ToList();
        if (commonTraits.Count > 0)
        {
            traits.Add(commonTraits[rng.Next(commonTraits.Count)]);
        }

        var skills = new List<SkillInstance>();
        if (_config.Skills.Count > 0)
        {
            skills.Add(new SkillInstance(_config.Skills[rng.Next(_config.Skills.Count)]));
        }

        return new Slime(Guid.NewGuid(), name, 1, stats, traits, skills, _config.BaseSplitIntervalSeconds);
    }

    public bool TryCreateOffspring(Slime parent, BreedingGround ground, Random rng, out SplitOutcome outcome)
    {
        var splitChance = CalculateSplitChance(parent, ground);
        if (rng.NextDouble() > splitChance)
        {
            outcome = default!;
            return false;
        }

        var child = CreateChild(parent, ground, rng, out var mutated, out var newTrait, out var newSkill);
        outcome = new SplitOutcome(parent, child, mutated, newTrait, newSkill);
        return true;
    }

    private Slime CreateChild(
        Slime parent,
        BreedingGround ground,
        Random rng,
        out bool mutated,
        out TraitDefinition? newTrait,
        out SkillDefinition? newSkill)
    {
        var mutationChance = CalculateMutationChance(parent, ground);
        mutated = rng.NextDouble() < mutationChance;

        var variance = mutated ? _config.MutationStatVariance : _config.StatVariance;
        var stats = parent.BaseStats.WithVariance(variance, rng);
        var generation = parent.Generation + 1;
        var childName = $"{parent.Name}-G{generation}-{rng.Next(100, 999)}";

        var traits = InheritTraits(parent, rng);
        newTrait = null;

        if (mutated && traits.Count < _config.MaxTraitsPerSlime)
        {
            newTrait = PickNewTrait(traits, parent, ground, rng);
            if (newTrait is not null)
            {
                traits.Add(newTrait);
            }
        }

        var skills = InheritSkills(parent, rng);
        newSkill = null;

        if ((mutated || rng.NextDouble() < 0.35) && skills.Count < _config.MaxSkillsPerSlime)
        {
            newSkill = PickNewSkill(skills, parent, ground, rng);
            if (newSkill is not null)
            {
                skills.Add(new SkillInstance(newSkill));
            }
        }

        var child = new Slime(
            Guid.NewGuid(),
            childName,
            generation,
            stats,
            traits,
            skills,
            parent.BaseSplitIntervalSeconds,
            parent.BaseMutationBonus,
            null);

        return child;
    }

    private List<TraitDefinition> InheritTraits(Slime parent, Random rng)
    {
        if (parent.Traits.Count == 0)
        {
            return new List<TraitDefinition>();
        }

        var inheritCount = Math.Min(parent.Traits.Count, _config.MaxTraitsPerSlime);
        return rng.TakeRandomSample(parent.Traits, inheritCount);
    }

    private List<SkillInstance> InheritSkills(Slime parent, Random rng)
    {
        if (parent.Skills.Count == 0)
        {
            return new List<SkillInstance>();
        }

        var inheritCount = Math.Min(parent.Skills.Count, _config.MaxSkillsPerSlime);
        var selected = rng.TakeRandomSample(parent.Skills, inheritCount);

        return selected
            .Select(instance => new SkillInstance(instance.Definition, instance.Level))
            .ToList();
    }

    private TraitDefinition? PickNewTrait(
        IReadOnlyCollection<TraitDefinition> existingTraits,
        Slime parent,
        BreedingGround ground,
        Random rng)
    {
        var picker = new WeightedPicker<TraitDefinition>();
        var takenIds = new HashSet<string>(existingTraits.Select(t => t.Id));

        foreach (var trait in _config.Traits)
        {
            if (takenIds.Contains(trait.Id))
            {
                continue;
            }

            var weight = 1.0 / (int)trait.Rarity;

            if (SharesTag(trait.Tags, ground.FavoredTraitTags))
            {
                weight *= _config.FavoredTraitWeightMultiplier;
            }

            if (parent.Accessory is { } accessory && SharesTag(trait.Tags, accessory.Definition.FavoredTraitTags))
            {
                weight *= _config.FavoredTraitWeightMultiplier;
            }

            picker.Add(trait, weight);
        }

        return picker.TryPick(rng, out var choice) ? choice : null;
    }

    private SkillDefinition? PickNewSkill(
        IReadOnlyCollection<SkillInstance> existingSkills,
        Slime parent,
        BreedingGround ground,
        Random rng)
    {
        var picker = new WeightedPicker<SkillDefinition>();
        var takenIds = new HashSet<string>(existingSkills.Select(s => s.Definition.Id));

        foreach (var skill in _config.Skills)
        {
            if (takenIds.Contains(skill.Id))
            {
                continue;
            }

            var weight = 1.0 / (int)skill.Rarity;

            if (ground.FavoredSkillType == skill.Type)
            {
                weight *= _config.FavoredSkillWeightMultiplier;
            }

            if (parent.Accessory is { } accessory && accessory.Definition.FavoredSkillType == skill.Type)
            {
                weight *= _config.FavoredSkillWeightMultiplier;
            }

            picker.Add(skill, weight);
        }

        return picker.TryPick(rng, out var choice) ? choice : null;
    }

    private double CalculateSplitChance(Slime parent, BreedingGround ground)
    {
        var effects = parent.CalculateAggregateEffects(ground.EnvironmentEffects);
        var chance = _config.BaseSplitChancePerCycle * effects.SplitSpeedMultiplier;
        return Clamp(chance, _config.SplitChanceFloor, _config.SplitChanceCap);
    }

    private double CalculateMutationChance(Slime parent, BreedingGround ground)
    {
        var effects = parent.CalculateAggregateEffects(ground.EnvironmentEffects);
        var chance = _config.BaseMutationChance + parent.BaseMutationBonus + effects.MutationChanceBonus;
        return Clamp(chance, _config.MutationChanceFloor, _config.MutationChanceCap);
    }

    private static bool SharesTag(IEnumerable<string> left, IEnumerable<string> right)
    {
        foreach (var tag in left)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                continue;
            }

            foreach (var other in right)
            {
                if (string.Equals(tag, other, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static double Clamp(double value, double min, double max)
        => Math.Max(min, Math.Min(max, value));
}
