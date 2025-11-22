using UniRx;
public interface IHealthViewModel
{
    IHealthModel Model { get; }
    ReactiveCommand OnDeath { get; }
    ReactiveCommand<float> TakeDamageCommand { get; }
    ReactiveCommand<float> SetHealthCommand { get; }
    ReactiveCommand<float> IncreaseHealthCommand { get; }
    bool IsDead { get; }
    void Dispose();
}
