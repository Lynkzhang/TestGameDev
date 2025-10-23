using System;

namespace SlimeEvolution.Core.Domain;

/// <summary>
/// Represents a collection of multiplicative and additive modifiers that combine
/// the influences of traits, accessories, environments and other systems.
/// </summary>
public readonly record struct EffectBundle(
    double SplitSpeedMultiplier = 1.0,
    double MutationChanceBonus = 0.0,
    double HpMultiplier = 1.0,
    double AttackMultiplier = 1.0,
    double DefenseMultiplier = 1.0,
    double SpeedMultiplier = 1.0,
    double SaleValueMultiplier = 1.0,
    double SaleValueBonus = 0.0)
{
    public static EffectBundle Identity { get; } = new();

    public EffectBundle Combine(EffectBundle other)
        => new(
            SplitSpeedMultiplier * other.SplitSpeedMultiplier,
            MutationChanceBonus + other.MutationChanceBonus,
            HpMultiplier * other.HpMultiplier,
            AttackMultiplier * other.AttackMultiplier,
            DefenseMultiplier * other.DefenseMultiplier,
            SpeedMultiplier * other.SpeedMultiplier,
            SaleValueMultiplier * other.SaleValueMultiplier,
            SaleValueBonus + other.SaleValueBonus);

    public EffectBundle Scale(double factor) => new(
        Math.Pow(SplitSpeedMultiplier, factor),
        MutationChanceBonus * factor,
        Math.Pow(HpMultiplier, factor),
        Math.Pow(AttackMultiplier, factor),
        Math.Pow(DefenseMultiplier, factor),
        Math.Pow(SpeedMultiplier, factor),
        Math.Pow(SaleValueMultiplier, factor),
        SaleValueBonus * factor);
}
