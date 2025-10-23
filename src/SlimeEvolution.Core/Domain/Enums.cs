namespace SlimeEvolution.Core.Domain;

public enum TraitRarity
{
    Common = 1,
    Uncommon = 2,
    Rare = 3,
    Epic = 4,
    Legendary = 5
}

public enum SkillType
{
    Attack,
    Defense,
    Support,
    Healing,
    Control
}

public enum SkillRarity
{
    Common = 1,
    Advanced = 2,
    Elite = 3,
    Mythic = 4
}

public enum AccessoryType
{
    Attribute,
    Bias,
    Special
}

public enum TaskCategory
{
    Daily,
    Achievement,
    Bounty,
    Story
}

public enum TaskTarget
{
    SplitSlimes,
    SellSlimes,
    WinBattles,
    AcquireTraits,
    AcquireSkills,
    ArchiveSlimes
}
