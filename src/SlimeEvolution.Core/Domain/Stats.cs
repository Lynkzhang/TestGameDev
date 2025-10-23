using System;

namespace SlimeEvolution.Core.Domain;

public readonly record struct Stats(int Hp, int Attack, int Defense, int Speed, int Mutation)
{
    public static Stats Zero { get; } = new(0, 0, 0, 0, 0);

    public int CombatScore => Attack + Defense + Speed;
    public int TotalScore => Hp + Attack + Defense + Speed + Mutation;

    public Stats ApplyEffects(EffectBundle effects)
        => new(
            Math.Max(1, (int)Math.Round(Hp * effects.HpMultiplier)),
            Math.Max(1, (int)Math.Round(Attack * effects.AttackMultiplier)),
            Math.Max(1, (int)Math.Round(Defense * effects.DefenseMultiplier)),
            Math.Max(1, (int)Math.Round(Speed * effects.SpeedMultiplier)),
            Math.Max(1, Mutation));

    public Stats WithVariance(double variance, Random rng)
    {
        return new Stats(
            ApplyVariance(Hp, variance, rng),
            ApplyVariance(Attack, variance, rng),
            ApplyVariance(Defense, variance, rng),
            ApplyVariance(Speed, variance, rng),
            ApplyVariance(Mutation, variance, rng));
    }

    private static int ApplyVariance(int value, double variance, Random rng)
    {
        if (variance <= 0 || value <= 0)
        {
            return Math.Max(1, value);
        }

        var delta = value * variance;
        var offset = (rng.NextDouble() * 2 - 1) * delta;
        return Math.Max(1, (int)Math.Round(value + offset));
    }
}

public readonly record struct StatRange(int Min, int Max)
{
    public int Roll(Random rng)
    {
        if (Min > Max)
        {
            throw new ArgumentException("Range minimum cannot exceed maximum.");
        }

        return rng.Next(Min, Max + 1);
    }
}

public sealed class StatDistribution
{
    public StatDistribution(StatRange hp, StatRange attack, StatRange defense, StatRange speed, StatRange mutation)
    {
        Hp = hp;
        Attack = attack;
        Defense = defense;
        Speed = speed;
        Mutation = mutation;
    }

    public StatRange Hp { get; }
    public StatRange Attack { get; }
    public StatRange Defense { get; }
    public StatRange Speed { get; }
    public StatRange Mutation { get; }

    public Stats Roll(Random rng)
        => new(
            Hp.Roll(rng),
            Attack.Roll(rng),
            Defense.Roll(rng),
            Speed.Roll(rng),
            Mutation.Roll(rng));
}

