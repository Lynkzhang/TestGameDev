using System;
using System.Linq;
using System.Text;
using SlimeEvolution.Core.Domain;

namespace SlimeEvolution.Cli;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var state = GameState.CreateDefault();

        var running = true;
        while (running)
        {
            ShowMainMenu(state);
            Console.Write("请选择操作: ");
            var input = Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    ShowBreedingGroundSummary(state);
                    break;
                case "2":
                    ListSlimes(state);
                    break;
                case "3":
                    RunSplitCycle(state);
                    break;
                case "4":
                    SellSlime(state);
                    break;
                case "5":
                    ShowLogs(state);
                    break;
                case "6":
                    running = false;
                    break;
                default:
                    Console.WriteLine("未识别的指令，请重新输入。");
                    break;
            }

            if (running)
            {
                Console.WriteLine();
                Console.WriteLine("按 Enter 继续...");
                Console.ReadLine();
            }
        }

        Console.WriteLine("感谢体验，欢迎继续关注《史莱姆进化》！");
    }

    private static void ShowMainMenu(GameState state)
    {
        Console.Clear();
        Console.WriteLine("==============================");
        Console.WriteLine("  史莱姆进化：文字试玩 (原型)");
        Console.WriteLine("==============================");
        Console.WriteLine($"金钱: {state.Gold}   晶石: {state.Crystal}");
        Console.WriteLine($"场地: {state.BreedingGround.Name}  容量 {state.BreedingGround.Slimes.Count}/{state.BreedingGround.Capacity}");
        Console.WriteLine($"累计分裂循环: {state.TotalSplitCycles}  诞生史莱姆: {state.TotalChildrenProduced}  变异数: {state.TotalMutations}");
        Console.WriteLine();
        Console.WriteLine("1. 查看场地概况");
        Console.WriteLine("2. 列出场地内的史莱姆");
        Console.WriteLine("3. 运行一次分裂循环");
        Console.WriteLine("4. 出售史莱姆换取金币");
        Console.WriteLine("5. 查看最近事件");
        Console.WriteLine("6. 退出试玩");
        Console.WriteLine();
    }

    private static void ShowBreedingGroundSummary(GameState state)
    {
        var ground = state.BreedingGround;
        Console.WriteLine("--- 场地概况 ---");
        Console.WriteLine($"名称：{ground.Name}");
        Console.WriteLine($"容量：{ground.Slimes.Count}/{ground.Capacity}");
        Console.WriteLine($"偏好特性：{string.Join('、', ground.FavoredTraitTags)}");
        Console.WriteLine($"偏好技能类型：{ground.FavoredSkillType?.ToString() ?? "无"}");
        Console.WriteLine("环境增益：");
        Console.WriteLine($"  分裂速度倍率：x{ground.EnvironmentEffects.SplitSpeedMultiplier:F2}");
        Console.WriteLine($"  变异概率加成：+{ground.EnvironmentEffects.MutationChanceBonus:P0}");
        Console.WriteLine($"  生命值倍率：x{ground.EnvironmentEffects.HpMultiplier:F2}");
        Console.WriteLine($"  出售价格倍率：x{ground.EnvironmentEffects.SaleValueMultiplier:F2}");
        Console.WriteLine();
    }

    private static void ListSlimes(GameState state)
    {
        var ground = state.BreedingGround;
        if (ground.Slimes.Count == 0)
        {
            Console.WriteLine("场地目前没有史莱姆。");
            return;
        }

        Console.WriteLine("--- 场地内史莱姆 ---");
        for (var i = 0; i < ground.Slimes.Count; i++)
        {
            var slime = ground.Slimes[i];
            var stats = slime.GetEffectiveStats(ground.EnvironmentEffects);
            var traits = slime.Traits.Count == 0
                ? "无"
                : string.Join('、', slime.Traits.Select(t => t.Name));
            var skills = slime.Skills.Count == 0
                ? "无"
                : string.Join('、', slime.Skills.Select(s => s.Definition.Name));

            Console.WriteLine($"[{i}] {slime.Name} (G{slime.Generation})");
            Console.WriteLine($"    HP:{stats.Hp} ATK:{stats.Attack} DEF:{stats.Defense} SPD:{stats.Speed} MUT:{stats.Mutation}");
            Console.WriteLine($"    特性：{traits}");
            Console.WriteLine($"    技能：{skills}");
            if (slime.Accessory is { } accessory)
            {
                Console.WriteLine($"    饰品：{accessory.Definition.Name}");
            }
        }
        Console.WriteLine();
    }

    private static void RunSplitCycle(GameState state)
    {
        var ground = state.BreedingGround;
        if (ground.Slimes.Count == 0)
        {
            Console.WriteLine("没有史莱姆可供分裂。");
            return;
        }

        var outcomes = ground.RunSplitCycle(state.MutationService, state.Rng);
        var mutationCount = outcomes.Count(o => o.WasMutation);
        state.RegisterSplitResults(1, outcomes.Count, mutationCount);

        if (outcomes.Count == 0)
        {
            state.AddLog("本次循环没有史莱姆成功分裂。");
            Console.WriteLine("本次循环没有史莱姆成功分裂。");
            return;
        }

        Console.WriteLine("--- 分裂结果 ---");
        foreach (var outcome in outcomes)
        {
            var message = $"{outcome.Parent.Name} 分裂出 {outcome.Child.Name} (变异: {outcome.WasMutation})";
            Console.WriteLine(message);
            state.AddLog(message);

            if (outcome.NewTrait is not null)
            {
                var traitMessage = $"  获得新特性：{outcome.NewTrait.Name}";
                Console.WriteLine(traitMessage);
                state.AddLog(traitMessage.Trim());
            }

            if (outcome.NewSkill is not null)
            {
                var skillMessage = $"  学习新技能：{outcome.NewSkill.Name}";
                Console.WriteLine(skillMessage);
                state.AddLog(skillMessage.Trim());
            }
        }
    }

    private static void SellSlime(GameState state)
    {
        var ground = state.BreedingGround;
        if (ground.Slimes.Count == 0)
        {
            Console.WriteLine("没有可出售的史莱姆。");
            return;
        }

        ListSlimes(state);
        Console.Write("请输入要出售的史莱姆编号：");
        var input = Console.ReadLine()?.Trim();
        if (!int.TryParse(input, out var index) || index < 0 || index >= ground.Slimes.Count)
        {
            Console.WriteLine("输入无效，操作取消。");
            return;
        }

        var slime = ground.Slimes[index];
        var price = state.EconomyService.CalculateSalePrice(slime, ground);
        ground.RemoveSlime(slime.Id);
        state.Gold += price;

        var message = $"出售 {slime.Name} 获得 {price} 金币。";
        Console.WriteLine(message);
        state.AddLog(message);
    }

    private static void ShowLogs(GameState state)
    {
        if (!state.Logs.Any())
        {
            Console.WriteLine("暂无事件记录。");
            return;
        }

        Console.WriteLine("--- 最近事件 ---");
        foreach (var entry in state.Logs)
        {
            Console.WriteLine(entry);
        }
    }
}
