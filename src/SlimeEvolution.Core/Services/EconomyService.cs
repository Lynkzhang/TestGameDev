using System;
using SlimeEvolution.Core.Configuration;
using SlimeEvolution.Core.Domain;

namespace SlimeEvolution.Core.Services;

public sealed class EconomyService
{
    private readonly GameBalanceConfig _config;

    public EconomyService(GameBalanceConfig config)
    {
        _config = config;
    }

    public int CalculateSalePrice(Slime slime, BreedingGround ground)
    {
        var economy = _config.Economy;
        var effects = slime.CalculateAggregateEffects(ground.EnvironmentEffects);
        var stats = slime.GetEffectiveStats(ground.EnvironmentEffects);

        double value = economy.BasePrice;
        value += (stats.Hp + stats.Attack + stats.Defense + stats.Speed) * economy.StatWeight;
        value += stats.Mutation * economy.MutationStatWeight;

        value *= GetTraitMultiplier(slime);
        value *= GetSkillMultiplier(slime);
        value *= 1.0 + (slime.Generation - 1) * economy.GenerationBonus;

        value *= effects.SaleValueMultiplier;
        value += effects.SaleValueBonus;

        return RoundToStep(value, economy.SaleRounding);
    }

    public int CalculateArchiveValue(Slime slime, BreedingGround ground)
    {
        var baseValue = CalculateSalePrice(slime, ground);
        var premium = _config.Economy.ArchivePremiumMultiplier;
        return RoundToStep(baseValue * premium, _config.Economy.SaleRounding);
    }

    private double GetTraitMultiplier(Slime slime)
    {
        double multiplier = 1.0;
        foreach (var trait in slime.Traits)
        {
            if (_config.Economy.TraitRarityMultipliers.TryGetValue(trait.Rarity, out var value))
            {
                multiplier *= value;
            }
        }

        return multiplier;
    }

    private double GetSkillMultiplier(Slime slime)
    {
        double multiplier = 1.0;
        foreach (var skill in slime.Skills)
        {
            if (_config.Economy.SkillRarityMultipliers.TryGetValue(skill.Definition.Rarity, out var value))
            {
                multiplier *= value;
            }
        }

        return multiplier;
    }

    private static int RoundToStep(double value, double step)
    {
        if (step <= 0)
        {
            return (int)Math.Round(value);
        }

        var rounded = Math.Round(value / step) * step;
        return (int)Math.Max(0, Math.Round(rounded));
    }
}
