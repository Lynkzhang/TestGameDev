using System;
using SlimeEvolution.Core.Domain;

namespace SlimeEvolution.Core.Tasks;

public sealed class TaskSystem
{
    public TaskProgress Evaluate(TaskDefinition definition, GameProgressSnapshot snapshot)
    {
        var current = definition.Target switch
        {
            TaskTarget.SplitSlimes => snapshot.TotalSplits,
            TaskTarget.SellSlimes => snapshot.SlimesSold,
            TaskTarget.WinBattles => snapshot.BattlesWon,
            TaskTarget.AcquireTraits => snapshot.TraitsAcquired,
            TaskTarget.AcquireSkills => snapshot.SkillsUnlocked,
            TaskTarget.ArchiveSlimes => snapshot.SlimesArchived,
            _ => 0
        };

        if (definition.Target == TaskTarget.AcquireTraits && definition.TargetTraitRarity is not null)
        {
            current = AdjustForRarity(current, snapshot.RareSlimesProduced, definition.TargetTraitRarity.Value);
        }

        var isCompleted = current >= definition.RequiredCount;
        return new TaskProgress(definition, Math.Min(current, definition.RequiredCount), isCompleted);
    }

    private static int AdjustForRarity(int baseline, int rareCount, TraitRarity rarity)
    {
        return rarity switch
        {
            TraitRarity.Rare => rareCount,
            TraitRarity.Epic or TraitRarity.Legendary => Math.Max(0, rareCount / 2),
            _ => baseline
        };
    }
}
