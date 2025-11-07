using UnityEngine;

public class AmmoModel
{
    public int CurrentAmmo { get; private set; }
    public int MaxAmmo { get; private set; }
    public AmmoModel(int maxAmmo = 30)
    {
        Debug.Log("AmmoModel successfuly injected");
        MaxAmmo = maxAmmo;
        CurrentAmmo = maxAmmo;
    }
    public void SetAmmo(int current, int max)
    {
        CurrentAmmo = Mathf.Clamp(current, 0, max);
        MaxAmmo = max;
    }
    public bool CanShoot() => CurrentAmmo > 0;
    public void Shoot()
    {
        if (CanShoot())
            CurrentAmmo--;
    }
    public void Reload(int amount)
    {
        CurrentAmmo = Mathf.Min(CurrentAmmo + amount, MaxAmmo);
    }
}
