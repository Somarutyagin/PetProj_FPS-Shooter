using UnityEngine;
using UniRx;

public class AmmoModel
{
    public ReactiveProperty<int> CurrentAmmo { get; } = new ReactiveProperty<int>();
    public ReactiveProperty<int> MaxAmmo { get; } = new ReactiveProperty<int>();

    public void SetAmmo(int current, int max)
    {
        CurrentAmmo.Value = Mathf.Clamp(current, 0, max);
        MaxAmmo.Value = max;
    }

    public void Shoot()
    {
        CurrentAmmo.Value--;
    }

    public void Reload(int amount)
    {
        CurrentAmmo.Value = Mathf.Min(CurrentAmmo.Value + amount, MaxAmmo.Value);
    }
}