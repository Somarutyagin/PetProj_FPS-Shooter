using System;
using UnityEngine;

public class AmmoViewModel
{
    public event Action<int, int> OnAmmoChanged;
    private readonly AmmoModel model;

    public AmmoViewModel(AmmoModel model)
    {
        Debug.Log("AmmoViewModel successfuly injected");
        this.model = model;
        NotifyView();
    }

    public void Shoot()
    {
        model.Shoot();
        NotifyView();
    }

    public void Reload(int amount)
    {
        model.Reload(amount);
        NotifyView();
    }

    public bool CanReload() => model.CurrentAmmo != model.MaxAmmo;
    public bool CanShoot() => model.CanShoot();

    private void NotifyView()
    {
        OnAmmoChanged?.Invoke(model.CurrentAmmo, model.MaxAmmo);
    }
}