using System;
using System.Collections.Generic;
using System.Linq;
using SlimeEvolution.Core.Services;

namespace SlimeEvolution.Core.Domain;

public sealed class BreedingGround
{
    private readonly List<Slime> _slimes;
    private readonly HashSet<string> _favoredTraitTags;

    public BreedingGround(
        string id,
        string name,
        int capacity,
        EffectBundle environmentEffects,
        IEnumerable<Slime>? initialSlimes = null,
        IEnumerable<string>? favoredTraitTags = null,
        SkillType? favoredSkillType = null)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
        }

        Id = id;
        Name = name;
        Capacity = capacity;
        EnvironmentEffects = environmentEffects;
        FavoredSkillType = favoredSkillType;
        _slimes = initialSlimes?.ToList() ?? new List<Slime>();
        _favoredTraitTags = favoredTraitTags is null ? new HashSet<string>() : new HashSet<string>(favoredTraitTags);
    }

    public string Id { get; }
    public string Name { get; }
    public int Capacity { get; private set; }
    public EffectBundle EnvironmentEffects { get; }
    public SkillType? FavoredSkillType { get; }
    public IReadOnlyCollection<string> FavoredTraitTags => _favoredTraitTags;
    public IReadOnlyList<Slime> Slimes => _slimes;
    public int AvailableSlots => Math.Max(0, Capacity - _slimes.Count);

    public bool TryAddSlime(Slime slime)
    {
        if (_slimes.Count >= Capacity)
        {
            return false;
        }

        _slimes.Add(slime);
        return true;
    }

    public bool RemoveSlime(Guid slimeId)
    {
        var index = _slimes.FindIndex(s => s.Id == slimeId);
        if (index >= 0)
        {
            _slimes.RemoveAt(index);
            return true;
        }

        return false;
    }

    public IReadOnlyList<SplitOutcome> RunSplitCycle(MutationService mutationService, Random rng)
    {
        var results = new List<SplitOutcome>();
        var snapshot = _slimes.ToList();

        foreach (var parent in snapshot)
        {
            if (_slimes.Count >= Capacity)
            {
                break;
            }

            if (mutationService.TryCreateOffspring(parent, this, rng, out var outcome))
            {
                _slimes.Add(outcome.Child);
                results.Add(outcome);
            }
        }

        return results;
    }

    public void ResizeCapacity(int newCapacity)
    {
        if (newCapacity < _slimes.Count)
        {
            throw new InvalidOperationException("Cannot reduce capacity below current slime count.");
        }

        Capacity = newCapacity;
    }
}

public sealed record SplitOutcome(
    Slime Parent,
    Slime Child,
    bool WasMutation,
    TraitDefinition? NewTrait,
    SkillDefinition? NewSkill);
