using Zenject;
using UniRx;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : IInitializable
{
    private readonly PlayerStatsConfig _config;
    private readonly Dictionary<StatType, ReactiveProperty<float>> _currentStats = new Dictionary<StatType, ReactiveProperty<float>>();

    public StatsManager(PlayerStatsConfig config)
    {
        _config = config;
    }

    [Inject]
    public void Initialize()
    {
        foreach (var stat in _config.Stats)
        {
            _currentStats[stat.Type] = new ReactiveProperty<float>(stat.BaseValue);
        }
    }

    public IReadOnlyReactiveProperty<float> GetStatPrecent(StatType type) => _currentStats[type];
    public float GetCurrentUpgrade(StatType type, Rarity rarity)
    {
        if (_currentStats.TryGetValue(type, out var stat))
        {
            var config = _config.Stats.Find(s => s.Type == type);
            if (config != null && stat.Value < config.MaxValue)
            {
                return Mathf.Min(stat.Value + config.UpgradeBaseIncrement + (int)rarity, config.MaxValue);
            }
        }

        //callback
        return 0;
    }

    public void UpgradeStat(StatType type, Rarity rarity)
    {
        if (_currentStats.TryGetValue(type, out var stat))
        {
            stat.Value = GetCurrentUpgrade(type, rarity);
        }
    }
}