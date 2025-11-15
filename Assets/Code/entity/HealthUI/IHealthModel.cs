using UniRx;
public interface IHealthModel
{
    ReactiveProperty<float> CurrentHealth { get; }
    float MaxHealth { get; }
    void SetHealth(float health);
}
