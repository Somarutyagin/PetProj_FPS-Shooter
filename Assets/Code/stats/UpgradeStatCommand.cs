public class UpgradeStatCommand
{
    private readonly StatsManager _statsManager;

    public UpgradeStatCommand(StatsManager statsManager)
    {
        _statsManager = statsManager;
    }

    public void Execute(StatType type, Rarity rarity)
    {
        _statsManager.UpgradeStat(type, rarity);
    }
}