using SlimeEvolution.Core.Domain;

namespace SlimeEvolution.Core.Tasks;

public sealed class TaskDefinition
{
    public TaskDefinition(
        string id,
        string title,
        string description,
        TaskCategory category,
        TaskTarget target,
        int requiredCount,
        TraitRarity? targetTraitRarity = null,
        SkillRarity? targetSkillRarity = null)
    {
        Id = id;
        Title = title;
        Description = description;
        Category = category;
        Target = target;
        RequiredCount = requiredCount;
        TargetTraitRarity = targetTraitRarity;
        TargetSkillRarity = targetSkillRarity;
    }

    public string Id { get; }
    public string Title { get; }
    public string Description { get; }
    public TaskCategory Category { get; }
    public TaskTarget Target { get; }
    public int RequiredCount { get; }
    public TraitRarity? TargetTraitRarity { get; }
    public SkillRarity? TargetSkillRarity { get; }
}

public sealed record TaskProgress(TaskDefinition Definition, int CurrentCount, bool IsCompleted);

public sealed class GameProgressSnapshot
{
    public int TotalSplits { get; init; }
    public int SlimesSold { get; init; }
    public int BattlesWon { get; init; }
    public int TraitsAcquired { get; init; }
    public int SkillsUnlocked { get; init; }
    public int SlimesArchived { get; init; }
    public int RareSlimesProduced { get; init; }
}
