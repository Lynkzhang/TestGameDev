using System.Collections.Generic;
using SlimeEvolution.Core.Domain;

namespace SlimeEvolution.Core.Configuration;

public static class GameDatabase
{
    public static GameBalanceConfig CreateDefault()
    {
        var config = new GameBalanceConfig();

        foreach (var trait in CreateTraits())
        {
            config.Traits.Add(trait);
        }

        foreach (var skill in CreateSkills())
        {
            config.Skills.Add(skill);
        }

        foreach (var accessory in CreateAccessories())
        {
            config.Accessories.Add(accessory);
        }

        return config;
    }

    private static IEnumerable<TraitDefinition> CreateTraits()
    {
        yield return new TraitDefinition(
            "quick-split",
            "加速分裂",
            "提高史莱姆的分裂效率。",
            TraitRarity.Common,
            new EffectBundle(splitSpeedMultiplier: 1.25),
            new[] { "speed", "split" });

        yield return new TraitDefinition(
            "iron-hide",
            "坚韧表皮",
            "拥有超乎寻常的防御力。",
            TraitRarity.Uncommon,
            new EffectBundle(hpMultiplier: 1.1, defenseMultiplier: 1.3),
            new[] { "defense" });

        yield return new TraitDefinition(
            "lucky-core",
            "幸运星",
            "幸运体质让变异和战利品更加丰厚。",
            TraitRarity.Rare,
            new EffectBundle(mutationChanceBonus: 0.08, saleValueMultiplier: 1.05),
            new[] { "luck", "mutation" });

        yield return new TraitDefinition(
            "rich-gel",
            "高产分泌",
            "分泌的粘液在市场上售价更高。",
            TraitRarity.Uncommon,
            new EffectBundle(saleValueMultiplier: 1.35, saleValueBonus: 20),
            new[] { "economy" });

        yield return new TraitDefinition(
            "arcane-spark",
            "奥术脉冲",
            "蕴含奥术能量，增加攻击并略微加速技能冷却。",
            TraitRarity.Epic,
            new EffectBundle(attackMultiplier: 1.4, speedMultiplier: 1.1),
            new[] { "arcane", "attack" });
    }

    private static IEnumerable<SkillDefinition> CreateSkills()
    {
        yield return new SkillDefinition(
            "slime-shot",
            "粘液喷射",
            "向敌人喷射具有腐蚀性的粘液。",
            SkillType.Attack,
            SkillRarity.Common,
            120,
            6,
            new[] { "projectile" });

        yield return new SkillDefinition(
            "mass-heal",
            "群体治愈",
            "治疗友方全体，恢复中量生命值。",
            SkillType.Healing,
            SkillRarity.Elite,
            0,
            12,
            new[] { "heal" });

        yield return new SkillDefinition(
            "shield-pulse",
            "护盾波动",
            "为一名友军提供可观的吸收护盾。",
            SkillType.Defense,
            SkillRarity.Advanced,
            0,
            10,
            new[] { "shield" });

        yield return new SkillDefinition(
            "corrosive-breath",
            "腐蚀吐息",
            "吐出腐蚀之息，持续降低敌人防御。",
            SkillType.Control,
            SkillRarity.Advanced,
            80,
            14,
            new[] { "debuff" });

        yield return new SkillDefinition(
            "time-gel",
            "时间凝胶",
            "短时间内加速我方全体行动速度。",
            SkillType.Support,
            SkillRarity.Mythic,
            0,
            18,
            new[] { "haste" });
    }

    private static IEnumerable<AccessoryDefinition> CreateAccessories()
    {
        yield return new AccessoryDefinition(
            "flame-essence",
            "火焰精华",
            "让史莱姆散发灼热气息，倾向于火焰变异。",
            AccessoryType.Bias,
            new EffectBundle(attackMultiplier: 1.2, splitSpeedMultiplier: 1.05),
            new[] { "fire", "attack" },
            SkillType.Attack);

        yield return new AccessoryDefinition(
            "frost-core",
            "寒冰晶核",
            "降低体温，增加防御与冰属性变异倾向。",
            AccessoryType.Bias,
            new EffectBundle(defenseMultiplier: 1.25, saleValueMultiplier: 1.05),
            new[] { "ice", "defense" },
            SkillType.Defense);

        yield return new AccessoryDefinition(
            "mystic-seed",
            "神秘灵种",
            "与森林奥秘共鸣，提高稀有变异概率。",
            AccessoryType.Special,
            new EffectBundle(mutationChanceBonus: 0.12, hpMultiplier: 1.1),
            new[] { "rare", "mutation" },
            SkillType.Support);

        yield return new AccessoryDefinition(
            "prosper-charm",
            "繁荣吊坠",
            "象征财富的吊坠，出售价格显著提升。",
            AccessoryType.Attribute,
            new EffectBundle(saleValueMultiplier: 1.6, saleValueBonus: 45));
    }
}
