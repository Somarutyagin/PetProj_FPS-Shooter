using Zenject;
using UniRx;

public class StatsViewModel : IInitializable
{
    private readonly StatsManager _statsManager;

    public StatsViewModel(StatsManager statsManager)
    {
        _statsManager = statsManager;
    }

    public void Initialize()
    {
    }

    public IReadOnlyReactiveProperty<float> GetSpeedValue() => _statsManager.GetStatPrecent(StatType.Speed);
    public IReadOnlyReactiveProperty<float> GetArmorValue() => _statsManager.GetStatPrecent(StatType.Armor);
    public IReadOnlyReactiveProperty<float> GetVampirismValue() => _statsManager.GetStatPrecent(StatType.Lifesteal);
}