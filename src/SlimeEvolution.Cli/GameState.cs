using System;
using System.Collections.Generic;
using SlimeEvolution.Core.Configuration;
using SlimeEvolution.Core.Domain;
using SlimeEvolution.Core.Services;

namespace SlimeEvolution.Cli;

internal sealed class GameState
{
    private readonly Queue<string> _log = new();

    private GameState(
        GameBalanceConfig config,
        MutationService mutationService,
        EconomyService economyService,
        BreedingGround breedingGround,
        Random rng)
    {
        Config = config;
        MutationService = mutationService;
        EconomyService = economyService;
        BreedingGround = breedingGround;
        Rng = rng;
    }

    public GameBalanceConfig Config { get; }
    public MutationService MutationService { get; }
    public EconomyService EconomyService { get; }
    public BreedingGround BreedingGround { get; }
    public Random Rng { get; }

    public int Gold { get; set; }
    public int Crystal { get; set; }
    public int TotalSplitCycles { get; private set; }
    public int TotalChildrenProduced { get; private set; }
    public int TotalMutations { get; private set; }

    public IReadOnlyCollection<string> Logs => _log;

    public void AddLog(string message)
    {
        var stamped = $"[{DateTime.Now:HH:mm:ss}] {message}";
        _log.Enqueue(stamped);
        while (_log.Count > 12)
        {
            _log.Dequeue();
        }
    }

    public void RegisterSplitResults(int cycles, int children, int mutations)
    {
        TotalSplitCycles += cycles;
        TotalChildrenProduced += children;
        TotalMutations += mutations;
    }

    public static GameState CreateDefault()
    {
        var config = GameDatabase.CreateDefault();
        var mutation = new MutationService(config);
        var economy = new EconomyService(config);
        var rng = new Random();

        var breedingGround = new BreedingGround(
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

        for (var i = 0; i < 3; i++)
        {
            var slime = mutation.CreateStarterSlime($"初始史莱姆{i + 1}", rng);
            breedingGround.TryAddSlime(slime);
        }

        var state = new GameState(config, mutation, economy, breedingGround, rng)
        {
            Gold = 120,
            Crystal = 5
        };

        state.AddLog("欢迎来到《史莱姆进化》原型场地！");
        state.AddLog("使用菜单操作培养场地，体验核心循环。");

        return state;
    }
}
