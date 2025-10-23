using System;
using System.Collections.Generic;
using SlimeEvolution.Core.Domain;

namespace SlimeEvolution.Core.Configuration;

public sealed class GameBalanceConfig
{
    public double BaseSplitChancePerCycle { get; set; } = 0.35;
    public double SplitChanceFloor { get; set; } = 0.05;
    public double SplitChanceCap { get; set; } = 0.95;

    public double BaseMutationChance { get; set; } = 0.12;
    public double MutationChanceFloor { get; set; } = 0.02;
    public double MutationChanceCap { get; set; } = 0.95;

    public double StatVariance { get; set; } = 0.15;
    public double MutationStatVariance { get; set; } = 0.25;

    public int MaxTraitsPerSlime { get; set; } = 3;
    public int MaxSkillsPerSlime { get; set; } = 3;

    public double FavoredTraitWeightMultiplier { get; set; } = 1.8;
    public double FavoredSkillWeightMultiplier { get; set; } = 1.6;

    public double BaseSplitIntervalSeconds { get; set; } = 60;

    public StatDistribution StarterStatDistribution { get; set; } = new(
        new StatRange(90, 120),
        new StatRange(20, 35),
        new StatRange(20, 35),
        new StatRange(10, 20),
        new StatRange(15, 25));

    public IList<TraitDefinition> Traits { get; } = new List<TraitDefinition>();
    public IList<SkillDefinition> Skills { get; } = new List<SkillDefinition>();
    public IList<AccessoryDefinition> Accessories { get; } = new List<AccessoryDefinition>();

    public EconomyConfig Economy { get; set; } = new();
}

public sealed class EconomyConfig
{
    public int BasePrice { get; set; } = 50;
    public double StatWeight { get; set; } = 0.6;
    public double MutationStatWeight { get; set; } = 1.0;
    public double GenerationBonus { get; set; } = 0.12;
    public double SaleRounding { get; set; } = 5;

    public IDictionary<TraitRarity, double> TraitRarityMultipliers { get; } = new Dictionary<TraitRarity, double>
    {
        { TraitRarity.Common, 1.0 },
        { TraitRarity.Uncommon, 1.15 },
        { TraitRarity.Rare, 1.35 },
        { TraitRarity.Epic, 1.65 },
        { TraitRarity.Legendary, 2.1 }
    };

    public IDictionary<SkillRarity, double> SkillRarityMultipliers { get; } = new Dictionary<SkillRarity, double>
    {
        { SkillRarity.Common, 1.0 },
        { SkillRarity.Advanced, 1.2 },
        { SkillRarity.Elite, 1.45 },
        { SkillRarity.Mythic, 1.8 }
    };

    public double ArchivePremiumMultiplier { get; set; } = 1.25;
}
