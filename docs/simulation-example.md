# 史莱姆进化核心系统示例

以下示例展示了如何在 C# 项目中使用 `SlimeEvolution.Core` 域层代码，完成一次简单的分裂循环并计算史莱姆的出售价值。

```csharp
using System;
using SlimeEvolution.Core.Configuration;
using SlimeEvolution.Core.Domain;
using SlimeEvolution.Core.Services;

var config = GameDatabase.CreateDefault();
var mutationService = new MutationService(config);
var economyService = new EconomyService(config);
var rng = new Random();

// 初始化培养场地（以“神秘森林”为例）
var mysticForest = new BreedingGround(
    id: "arena-mystic",
    name: "神秘森林",
    capacity: 12,
    environmentEffects: new EffectBundle(
        splitSpeedMultiplier: 1.1,
        mutationChanceBonus: 0.05,
        hpMultiplier: 1.05,
        attackMultiplier: 1.0,
        defenseMultiplier: 1.0,
        speedMultiplier: 1.0,
        saleValueMultiplier: 1.05),
    favoredTraitTags: new[] { "rare", "mutation" },
    favoredSkillType: SkillType.Support);

// 添加初始史莱姆
for (int i = 0; i < 3; i++)
{
    var starter = mutationService.CreateStarterSlime($"初始史莱姆{i + 1}", rng);
    mysticForest.TryAddSlime(starter);
}

// 运行一次分裂循环
var outcomes = mysticForest.RunSplitCycle(mutationService, rng);

foreach (var outcome in outcomes)
{
    Console.WriteLine($"{outcome.Parent.Name} 分裂出 {outcome.Child.Name} (变异: {outcome.WasMutation})");
    if (outcome.NewTrait is not null)
    {
        Console.WriteLine($"  获得新特性: {outcome.NewTrait.Name}");
    }
    if (outcome.NewSkill is not null)
    {
        Console.WriteLine($"  学习新技能: {outcome.NewSkill.Name}");
    }

    var salePrice = economyService.CalculateSalePrice(outcome.Child, mysticForest);
    Console.WriteLine($"  当前出售价格: {salePrice}");
}
```

运行结果示例：

```
初始史莱姆1 分裂出 初始史莱姆1-G2-341 (变异: True)
  获得新特性: 幸运星
  学习新技能: 时间凝胶
  当前出售价格: 486
初始史莱姆3 分裂出 初始史莱姆3-G2-672 (变异: False)
  当前出售价格: 295
```

> 提示：在 Unity 项目中，可以将 `GameDatabase.CreateDefault()` 的数据迁移到
> `ScriptableObject` 或配置文件中，以便于策划与平衡调整。
```