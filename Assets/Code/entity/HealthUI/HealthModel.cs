using UniRx;
using UnityEngine;

public class HealthModel : IHealthModel
{
    public ReactiveProperty<float> CurrentHealth { get; } = new ReactiveProperty<float>();
    public float MaxHealth { get; }
    public HealthModel(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth.Value = maxHealth;
    }
    public void SetHealth(float health)
    {
        CurrentHealth.Value = Mathf.Clamp(health, 0, MaxHealth);
    }
}
