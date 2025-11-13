using UnityEngine;

public class AmmoModel
{
    public int CurrentAmmo { get; private set; }
    public int MaxAmmo { get; private set; }
    public void SetAmmo(int current, int max)
    {
        CurrentAmmo = Mathf.Clamp(current, 0, max);
        MaxAmmo = max;
    }
    public void Shoot()
    {
        CurrentAmmo--;
    }
    public void Reload(int amount)
    {
        CurrentAmmo = Mathf.Min(CurrentAmmo + amount, MaxAmmo);
    }
}
